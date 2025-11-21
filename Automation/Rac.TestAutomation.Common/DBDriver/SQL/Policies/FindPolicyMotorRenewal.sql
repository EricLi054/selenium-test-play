/*
PURPOSE: Fetch motor policies in renewal based from payment term, collection method and motor cover.

AUTHOR: Unknown
LAST UPDATED: Lopa Lenka 2024-05-01

TODO: RAI-239 Remove comment duplication from SQL Queries
*/
SELECT TOP 40 ppc.contact_id as ContactId,
    p.EXTERNAL_POLICY_NUMBER as PolicyNumber,
    case when CHARINDEX('-', tplo.description) > 0
            then SUBSTRING(tplo.description, 1, CHARINDEX('-', tplo.description)-1) 
		    else tplo.description end CoverType,
    (select tpv.VERSION_NR from T_PRODUCT_VERSION tpv where tpv.id = p.PRODUCT_VERSION_ID) AS ProductVersionNumber
  FROM p_policy p
  JOIN p_pol_header ph           ON p.id = ph.active_policy_id
  JOIN p_policy_contact ppc      ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
  join cn_person cnp             ON cnp.contact_id = ppc.contact_id
  join p_cover  pc               ON p.id=pc.ENDORSMENT_ID and pc.PARENT_COVER_ID is null -- because we retrieve cover, we only want the parent cover
  join t_product_line_option tplo       on pc.product_option_id = tplo.id
  join T_PRODUCT_LINE_OPTION_TYPE tplot on tplot.id = tplo.option_type_id
 WHERE ph.product_id           = 1000000 -- Motor Policy
  AND p.ENDORS_START_DATE     > convert(date, GETDATE())
  AND ph.status_renewal_date  > DATEADD(day, 1, convert(date, GETDATE()))
  AND PH.MAIN_RENEWAL_DATE    > ph.status_renewal_date
  AND PH.MANUAL_NON_RENEWALS_CODE is null --This section is to avoid policies that are flagged for manual non-renewal which would put them in a "Proposal For Renewal (PFR)" state on renewal
  AND p.status_ID             = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ph.status_ID            = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND p.ENDORSMENT_TYPE_ID    = 10 -- 10 Policy Renewal
  AND ppc.policy_contact_role in (6,8) -- 6=Policyholder 8=co-Policyholder
  AND ph.STATUS_RENEWAL_DATE IS NOT NULL
  AND p.collection_method_ID  = @collectionmethod -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
  AND p.payment_term_id       = @paymentterm -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
  AND cnp.date_of_birth is not null -- helps screen out company policies.
  and tplot.id                = @motorcover -- 1000013 is MFCO cover | 1000017 is MTFT cover | 1000018 is TPO cover
ORDER BY newid();