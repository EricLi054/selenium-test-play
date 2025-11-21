/*
PURPOSE: Fetch any current motor policy with the following filters:
      - Payment method based from payment term and collection method
	  - Either have no Annual KM or set to 10,000km
	  - not financed and has no modifications
	  - returns either PH or coPH.
AUTHOR:  Unknown
LAST UPDATE: Lopa Lenka 2024-05-01

TODO: RAI-239 Remove comment duplication from SQL Queries
*/
SELECT TOP 500
  c.ID as ContactId,
  p.EXTERNAL_POLICY_NUMBER as PolicyNumber,
  tkpy.description as KM_PER_YEAR,
  (select tpv.VERSION_NR from T_PRODUCT_VERSION tpv where tpv.id = p.PRODUCT_VERSION_ID) AS ProductVersionNumber,
  tplo.description as Cover_Type
FROM p_policy p
JOIN p_pol_header ph                  ON ph.active_policy_id = p.id
JOIN p_policy_contact ppc             ON ppc.policy_id = PH.ACTIVE_POLICY_ID
JOIN cn_contact c                     ON c.id = ppc.contact_id
JOIN cn_person cnp                    ON cnp.contact_id = ppc.contact_id
JOIN p_policy_lob ppl                 ON PH.ACTIVE_POLICY_ID = ppl.policy_id
JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA ON PPL.ID = PPLTLA.POLICY_LOB_ID
JOIN P_POLICY_LOB_ASSET PPLA          ON PPLTLA.LOB_ASSET_ID = PPLA.ID
JOIN AS_Asset ass                     ON PPLA.LOB_ASSET_ID = ass.id
JOIN AS_Vehicle asv                   ON ass.id = asv.Asset_id
JOIN AS_vehicle_raci asvr             ON asvr.id =asv.id
JOIN p_cover  pc					  ON p.id=pc.ENDORSMENT_ID
JOIN t_product_line_option tplo       on pc.product_option_id = tplo.id
LEFT JOIN    cn_address ca            ON ca.ID = asvr.address_id
LEFT JOIN    cn_address_raci car      ON car.ID = ca.ID
FULL OUTER JOIN T_KM_PER_YEAR tkpy				ON tkpy.id = asv.km_per_year_id
WHERE ph.product_id           = 1000000 -- Motor Policy
  AND ph.policy_end_date      between DATEADD(month, 1, convert(date, GETDATE())) and DATEADD(month, 11, convert(date, GETDATE()))
  AND ph.status_ID            = 20     -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ppc.policy_contact_role in (6,8) -- 6=Policyholder 8=co-Policyholder
  AND p.collection_method_ID  = @collectionmethod -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
  AND p.payment_term_id       = @paymentterm -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
  AND cnp.date_of_birth       is not null -- helps screen out company policies.
  AND (asv.km_per_year_id is NULL OR asv.km_per_year_id = 3000001) -- Null or 'Up to 10,000' km only.
  AND ass.MORTGAGE_TYPE_ID = 3 --No
  AND asvr.modification_type_id = 1000001 --No Mods
  AND (@needsQAS=0 OR car.IS_QAS_VALIDATED = 1) -- Only retrieves QAS Validated address
  and tplo.option_type_id in (1000013, 1000017, 1000018)
ORDER BY newid();