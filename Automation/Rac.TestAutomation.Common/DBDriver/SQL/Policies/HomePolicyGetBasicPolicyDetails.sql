/*
PURPOSE: Fetch basic home policy details.

AUTHOR: Unknown
LAST UPDATED: Jason King 2021-09-03
*/
SELECT    FORMAT(p.policy_start_date,'d MMMM yyyy') as term_start,
          tpot.description as Occupation,
		  tplo.description,
          FORMAT(pc.excess_amn, '0') as Excess,
          FORMAT(pc.ins_amn, '0')    as SI,
          tplot.external_code        as CoverCode,
		  aspr.WEEKLY_RENTAL_AMOUNT,
		 (select tpmr.DESCRIPTION from T_PROPERTY_MANAGER_RACI tpmr where tpmr.ID = aspr.PROPERTY_MANAGER) as PropertyMgr
   FROM p_policy p 
   JOIN p_pol_header ph         ON ph.active_policy_id = p.id 
   JOIN p_policy_lob ppl        ON PH.ACTIVE_POLICY_ID = ppl.policy_id 
   JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA   ON PPL.ID = PPLTLA.POLICY_LOB_ID 
   JOIN P_POLICY_LOB_ASSET PPLA ON PPLTLA.LOB_ASSET_ID = PPLA.ID 
   JOIN AS_Asset ass            ON PPLA.LOB_ASSET_ID = ass.id 
   JOIN AS_Property asp         ON ass.id = asp.ASSET_ID 
   JOIN  AS_property_raci aspr  ON  aspr.id = asp.ASSET_ID
   JOIN p_cover pc              ON ph.ACTIVE_POLICY_ID = pc.ENDORSMENT_ID 
   JOIN T_PROPERTY_OCCUPATION_TYPE tpot    ON tpot.id = asp.PROPERTY_OCCUPATION_TYPE 
   JOIN t_product_line_option_amount TPLOA ON PC.Excess_Level_NR = TPLOA.id 
   join t_product_line_option tplo         ON pc.product_option_id = tplo.id 
   join T_PRODUCT_LINE_OPTION_TYPE tplot   ON tplot.id = tplo.option_type_id
   WHERE p.external_policy_number = @policynumber 
     AND pc.PARENT_COVER_ID is null
ORDER BY tplo.description asc;