/*
PURPOSE: Fetch list of disclosed claims from when the policy
         was purchased.
AUTHOR: Jason King
LAST UPDATE: 2021-09-01
*/
SELECT (select cType.DESCRIPTION from T_HOME_CLAIM_TYPE_RACI cType where cType.ID = claimHis.CLAIM_TYPE) as Type,
	   claimHis.CLAIMED_YEARS as Year
FROM   p_policy p  
JOIN   p_pol_header ph ON  PH.ACTIVE_POLICY_ID = p.id  
JOIN  p_policy_lob ppl                    ON  PH.ACTIVE_POLICY_ID = ppl.policy_id
JOIN  P_POLICY_LOB_TO_LOB_ASSET PPLTLA    ON  PPL.ID = PPLTLA.POLICY_LOB_ID
JOIN  P_POLICY_LOB_ASSET PPLA             ON  PPLTLA.LOB_ASSET_ID = PPLA.ID
JOIN  AS_Property asp                     ON  PPLA.LOB_ASSET_ID = asp.ASSET_ID
LEFT JOIN  AS_HOME_CLAIM_HISTORY_RACI claimHis ON  claimHis.ASSET_ID = asp.ASSET_ID
where p.EXTERNAL_POLICY_NUMBER = @policynumber
;