/*
PURPOSE: Fetch a current motor policy (MFCO cover) with a high SI and high risk area based from payment term and collection method
AUTHOR:  Unknown
LAST UPDATE: Lopa Lenka 2024-05-01

TODO: RAI-239 Remove comment duplication from SQL Queries
*/
SELECT TOP 500
  ppc.CONTACT_ID as ContactId,
  p.EXTERNAL_POLICY_NUMBER as PolicyNumber,
  tkpy.description as KM_PER_YEAR,
  (select tpv.VERSION_NR from T_PRODUCT_VERSION tpv where tpv.id = p.PRODUCT_VERSION_ID) AS ProductVersionNumber,
  tplo.description as Cover_Type
FROM p_policy p
JOIN p_pol_header ph                  ON ph.active_policy_id = p.id
JOIN p_policy_contact ppc             ON ppc.policy_id = PH.ACTIVE_POLICY_ID
JOIN cn_person cnp                    ON cnp.contact_id = ppc.contact_id
JOIN CN_CONTACT_RACI cnr              ON cnr.id = cnp.contact_id
JOIN t_collection_method tcm          ON tcm.id = p.COLLECTION_METHOD_ID
JOIN t_payment_terms tpt              ON tpt.id = p.payment_term_id
JOIN p_cover  pc                      ON p.id=pc.ENDORSMENT_ID and pc.PARENT_COVER_ID is null
JOIN t_product_line_option tplo       on pc.product_option_id = tplo.id
JOIN T_PRODUCT_LINE_OPTION_TYPE tplot on tplot.id = tplo.option_type_id
JOIN p_policy_lob ppl                 ON PH.ACTIVE_POLICY_ID = ppl.policy_id
JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA ON PPL.ID = PPLTLA.POLICY_LOB_ID
JOIN P_POLICY_LOB_ASSET_RACI PLAR     ON PPLTLA.LOB_ASSET_ID = plar.id
JOIN P_POLICY_LOB_ASSET PPLA          ON PPLTLA.LOB_ASSET_ID = PPLA.ID
JOIN t_asset_risk tar                 ON tar.id = plar.city_id  
JOIN AS_Asset ass                     ON PPLA.LOB_ASSET_ID = ass.id 
JOIN AS_Vehicle asv                   ON ass.id = asv.Asset_id
JOIN T_KM_PER_YEAR tkpy               ON tkpy.id = asv.km_per_year_id
JOIN AS_vehicle_raci asvr             ON asvr.id =asv.id
LEFT JOIN    cn_address ca            ON ca.ID = asvr.address_id
LEFT JOIN    cn_address_raci car      ON car.ID = ca.ID
WHERE ph.product_id           = 1000000 -- Motor
  AND p.policy_start_date < DATEADD(month, -2, convert(date, GETDATE()))
  AND p.policy_end_date   > DATEADD(month, 3, convert(date, GETDATE()))
  AND p.status_ID             = 20      -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  and tplo.option_type_id     = 1000013 -- 1000013 is MFCO cover | 1000017 is MTFT cover | 1000018 is TPO cover
  AND ppc.policy_contact_role in (6,8)  -- 6=Policyholder 8=co-Policyholder
  AND p.collection_method_ID  = @collectionmethod -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
  AND p.payment_term_id       = @paymentterm -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
  AND cnp.date_of_birth is not null -- helps screen out company policies.
  AND asv.BASIC_SUR_AMT > 40000     -- vehicle market value over $40k
  AND tar.risk_value > 50
  -- Tests do not always override KM Per Year value, so need policies where a valid value is defined
  AND asv.km_per_year_id != 3000005 -- T_KM_PER_YEAR.ID = 3000005 DESCRIPTION = 'Support Only'
  -- avoid contacts with bad tokens in case of refund to source to credit card
  AND ((p.collection_method_ID = 2) OR (cnr.WESTPAC_CUSTOMER_ID = cnp.contact_id))
  AND (@needsQAS=0 OR car.IS_QAS_VALIDATED = 1) -- Only retrieves QAS Validated address
ORDER BY newid();