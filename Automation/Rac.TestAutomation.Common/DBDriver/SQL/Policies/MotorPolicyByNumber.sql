SELECT format(p.policy_start_date,'d/MMM/yyyy') as term_start,
       tplo.description as Cover,
	   floor(pc.excess_amn) as Excess,
	   floor(asv.BASIC_SUR_AMT + (select pa.amount from p_amount pa where pa.ID = asv.TOTAL_ACCESSORIES_VALUE)) as SI,
	   round(p.YEARLY_PREMIUM,2,0) as Premium
  FROM p_pol_header ph 
  JOIN p_policy p ON ph.active_policy_id = p.id 
  JOIN p_cover pc ON ph.ACTIVE_POLICY_ID = pc.ENDORSMENT_ID 
  JOIN T_PRODUCT_LINE_OPTION tplo ON pc.PRODUCT_OPTION_ID = tplo.ID 
  JOIN p_policy_lob ppl                 ON PH.ACTIVE_POLICY_ID = ppl.policy_id
  JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA ON PPL.ID = PPLTLA.POLICY_LOB_ID
  JOIN P_POLICY_LOB_ASSET PPLA          ON PPLTLA.LOB_ASSET_ID = PPLA.ID
  JOIN AS_Asset ass                     ON PPLA.LOB_ASSET_ID = ass.id
  JOIN AS_Vehicle asv                   ON ass.id = asv.Asset_id
 WHERE pc.PARENT_COVER_ID is null AND p.external_policy_number = @policynumber
ORDER BY tplo.description asc
