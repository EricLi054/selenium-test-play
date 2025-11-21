/*
PURPOSE: Fetch contents items relating to claim.

AUTHOR: Jason King
LAST UPDATED: Jason King 2022-06-29
*/
SELECT  ASSI.REMARKS,
        FORMAT(COALESCE(ASSI.item_amount, 
		                       (select pa.amount from P_AMOUNT pa where pa.ID = ASSI.ITEM_AMOUNT_ID), 
							   0), '0') as Value,
		(select type.description from T_ASSET_ITEM_TYPE type where type.ID = assi.ITEM_TYPE_ID) as valuableType
FROM          C_claim cl                        
        JOIN  C_CLAIM_CONTACT CC              ON  Cl.ID = CC.CLAIM_ID
        JOIN  C_CLAIMANT CT                   ON  CC.ID = CT.ID 
        JOIN  C_CLAIMANT_ASSET CCA            ON  CT.ID = CCA.CLAIMANT_ID
		JOIN  C_CLAIMANT_ASSET_DAMAGED_ITEM IT ON IT.CLAIMANT_ASSET_ID = CCA.ID
		JOIN  AS_ASSET_ITEM ASSI              ON  IT.ASSET_ITEM_ID = ASSI.ID
WHERE   cl.claim_number = @ClaimNumber
;
