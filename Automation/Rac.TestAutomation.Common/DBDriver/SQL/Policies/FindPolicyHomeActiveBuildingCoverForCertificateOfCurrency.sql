SELECT TOP 2
p.EXTERNAL_POLICY_NUMBER

FROM p_policy p
JOIN p_pol_header ph      ON ph.active_policy_id = p.id
JOIN p_policy_contact ppc ON ppc.policy_id = PH.ACTIVE_POLICY_ID
JOIN cn_person cnp        ON cnp.contact_id = ppc.contact_id

WHERE ph.product_id           = 1000001 -- Home Product
  AND ph.status_ID            = 20     -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ppc.policy_contact_role in (6,8) -- 6=Policyholder 8=co-Policyholder
  AND (
		SELECT 
		  count(*) 
		from 
		  p_cover pc 
		  join t_product_line_option tplo on pc.product_option_id = tplo.id 
		  join T_PRODUCT_LINE_OPTION_TYPE tplot on tplot.id = tplo.option_type_id
		 WHERE pc.PARENT_COVER_ID is null 
		   AND p.id=pc.ENDORSMENT_ID 
		   AND tplot.external_code in ('HB','LB') 
		   AND pc.COVER_STATUS_ID = 20
      ) = 1  -- Has active cover for a Building (either Homeowner or Landlords)
  -- Avoid being in renewal state
  AND p.policy_end_date between DATEADD(DAY, 40, convert(date, GETDATE())) and DATEADD(DAY, 320, convert(date, GETDATE()))