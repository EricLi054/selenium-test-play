/*
PURPOSE: Fetch all specified valuables and contents for a
         given policy. The category (tati.description)
         designates if an item is contents or SPV.
AUTHOR: Jason King
LAST UPDATE: 2022-06-29
*/
SELECT tati.description as SPV_Category,
       aai.remarks as Item_description,
       FLOOR(COALESCE(aai.item_amount, 
		              (select pa.amount from P_AMOUNT pa where pa.ID = aai.ITEM_AMOUNT_ID), 
					  0)) as value

FROM p_policy p 
JOIN p_pol_header ph ON ph.active_policy_id = p.id 
JOIN p_policy_lob ppl ON PH.ACTIVE_POLICY_ID = ppl.policy_id 
JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA ON PPL.ID = PPLTLA.POLICY_LOB_ID 
JOIN P_POLICY_LOB_ASSET PPLA ON PPLTLA.LOB_ASSET_ID = PPLA.ID 
JOIN P_POLICY_LOB_ASSET_RACI PLAR ON ppla.ID = plar.id 
JOIN AS_Asset ass ON PPLA.LOB_ASSET_ID = ass.id 
JOIN as_asset_item aai ON ass.id = aai.asset_id 
JOIN T_ASSET_ITEM_TYPE tati ON tati.id = aai.ITEM_TYPE_ID 
WHERE p.external_policy_number = @policynumber 
AND aai.for_claims_only = 0 
AND aai.POLICY_TOTAL_LOSS = 0 
AND aai.CLAIM_TOTAL_LOSS = 0