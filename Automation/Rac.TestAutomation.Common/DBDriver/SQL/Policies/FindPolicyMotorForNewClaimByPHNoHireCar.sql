/*
PURPOSE: Find a contact with a current Motor Policy where they are the PH,
cover is full comprehensive, insured value of the vehicle is over $10,000,
and there have been no claims in the past 2 months.
Policy should also NOT have hire car cover.
AUTHOR: Unknown
LAST UPDATE: Jason King 2021-07-15
*/
SELECT TOP 40 p.EXTERNAL_POLICY_NUMBER,
         (select c.first_name from cn_contact c where ppc.contact_id = c.id),
         (select c.name from cn_contact c where ppc.contact_id = c.id),
		 format(cnp.date_of_birth,'dd/MM/yyyy'),
         ppc.contact_id
  FROM p_policy p
  JOIN p_pol_header ph           ON p.id = ph.active_policy_id
  JOIN p_policy_contact ppc      ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
  JOIN cn_person cnp             ON cnp.contact_id = ppc.contact_id
  JOIN p_cover  pc               ON p.id=pc.ENDORSMENT_ID
  WHERE ph.product_id           = 1000000 -- Motor Policy
  AND PH.MAIN_RENEWAL_DATE    > ph.status_renewal_date
  AND p.status_ID             = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ph.status_ID            = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND (select tplo.external_code from t_product_line_option tplo where pc.product_option_id = tplo.id) = 'MFCO'
  AND (                 -- Does not have MHAO (hire car) cover
    select count(*)
    from p_cover  pc2 join t_product_line_option tplo2 on pc2.product_option_id = tplo2.id
    where p.id=pc2.ENDORSMENT_ID AND tplo2.external_code = 'MHAO') = 0
  AND p.policy_end_date   between DATEADD(month, 1, convert(date, GETDATE())) and DATEADD(month, 10, convert(date, GETDATE())) 
  AND cnp.date_of_birth is not null -- helps screen out company policies.
  AND ppc.policy_contact_role = 6 -- 8=co-PH. 6=PH, 1000000=auth party. We only want PH.  
  -- Vehicle value above $10,000
  AND ( select TOP 1 asv.basic_sur_amt from p_policy_lob ppl 
        JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA ON PPL.ID = PPLTLA.POLICY_LOB_ID
        JOIN AS_Vehicle asv                   ON PPLTLA.LOB_ASSET_ID = asv.Asset_id 
        where PH.ACTIVE_POLICY_ID = ppl.policy_id) > 10000
  -- Want Private usage as that will block payments.
  AND  (select tvu.external_code from p_policy_lob ppl
		  JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA ON PPL.ID = PPLTLA.POLICY_LOB_ID
          JOIN P_POLICY_LOB_ASSET PPLA          ON PPLTLA.LOB_ASSET_ID = PPLA.ID
          JOIN AS_Asset ass                     ON PPLA.LOB_ASSET_ID = ass.id
          JOIN AS_Vehicle asv                   ON ass.id = asv.Asset_id
          JOIN T_VEHICLE_USAGE tvu              ON tvu.id = asv.use_id
		  WHERE PH.ACTIVE_POLICY_ID = ppl.policy_id) = 'Private'
ORDER BY newid()