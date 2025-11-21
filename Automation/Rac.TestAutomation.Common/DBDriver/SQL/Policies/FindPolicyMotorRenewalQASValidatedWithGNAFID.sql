/*
PURPOSE:Retrieve motor policies that are QAS-validated and have a non-null GNAF ID for renewal based on payment term, collection method, and motor cover.
AUTHOR: Christine Entrada 23-02-2023
LAST UPDATED: Lopa Lenka 2024-05-01
*/
SELECT TOP 50 ppc.contact_id ContactId,
    p.EXTERNAL_POLICY_NUMBER as PolicyNumber,
    case when CHARINDEX('-', tplo.description) > 0
            then SUBSTRING(tplo.description, 1, CHARINDEX('-', tplo.description)-1) 
		    else tplo.description end CoverType,
    (select tpv.VERSION_NR from T_PRODUCT_VERSION tpv where tpv.id = p.PRODUCT_VERSION_ID) AS ProductVersionNumber
  FROM p_policy p
  JOIN p_pol_header ph						ON p.id = ph.active_policy_id
  JOIN p_policy_contact ppc					ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
  join cn_person cnp						ON cnp.contact_id = ppc.contact_id
  join p_cover  pc							ON p.id=pc.ENDORSMENT_ID and pc.PARENT_COVER_ID is null -- because we retrieve cover, we only want the parent cover
  join t_product_line_option tplo			ON pc.product_option_id = tplo.id
  JOIN p_policy_lob ppl						ON ph.active_policy_id = ppl.policy_id
  JOIN p_policy_lob_to_lob_asset ppltla		ON ppl.ID = ppltla.policy_lob_id
  JOIN p_policy_lob_asset ppla				ON ppltla.lob_asset_id = ppla.ID
  JOIN as_asset ass							ON ppla.lob_asset_id = ass.ID
  JOIN as_vehicle asv						ON ass.ID = asv.asset_id
  join T_PRODUCT_LINE_OPTION_TYPE tplot		ON tplot.id = tplo.option_type_id
  JOIN as_vehicle_raci asvr					ON asv.ID = asvr.ID
  LEFT JOIN    cn_address ca                ON ca.ID = asvr.address_id
  LEFT JOIN    cn_address_raci car          ON car.ID = ca.ID
 WHERE ph.product_id           = 1000000 -- Motor Policy
  AND p.ENDORS_START_DATE     > convert(date, GETDATE())
  AND ph.status_renewal_date  > DATEADD(day, 1, convert(date, GETDATE()))
  AND PH.MAIN_RENEWAL_DATE    > ph.status_renewal_date
  AND ph.MANUAL_NON_RENEWALS_CODE is null -- This section is to avoid policies that are flagged for manual non-renewal which would put them in a "Proposal For Renewal (PFR)" state on renewal
  AND p.status_ID             = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ph.status_ID            = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND p.ENDORSMENT_TYPE_ID    = 10 -- 10 Policy Renewal
  AND ppc.policy_contact_role in (6,8) -- 6=Policyholder 8=co-Policyholder
  AND ph.STATUS_RENEWAL_DATE IS NOT NULL 
  AND car.IS_QAS_VALIDATED = 1 -- Only retrieves QAS Validated address
  AND car.GNAF_PID_REF_ID IS not null -- Only retrieves Policies with GNAF ID
  AND p.collection_method_ID  = @collectionmethod -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
  AND p.payment_term_id       = @paymentterm -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
  AND cnp.date_of_birth is not null -- helps screen out company policies.
  and tplot.id                = @motorcover -- 1000013 is MFCO cover | 1000017 is MTFT cover | 1000018 is TPO cover
ORDER BY newid();