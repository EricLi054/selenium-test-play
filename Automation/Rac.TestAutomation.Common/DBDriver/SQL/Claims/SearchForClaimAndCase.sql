/*
PURPOSE: Fetch claim having case linked or not linked depending on the flag 'isCaseLinkedClaim'
isCaseLinkedClaim equals to 0 will return claim having case number
isCaseLinkedClaim equals to 1 will return claim NOT having case number
AUTHOR: Hashili
LAST UPDATED: 2025-Sep-17
*/
SELECT TOP (2) 
    CC.ID as ClaimNumber,
    LLE1.Case_Number as CaseNumber,
    P.EXTERNAL_POLICY_NUMBER as PolicyNumber
FROM C_CLAIM CC
OUTER APPLY (
    SELECT TOP (1) LLE.LITIGATION_ID AS Case_Number
    FROM L_LITIGATION_ENTITY LLE
    WHERE LLE.ENTITY_ID = CC.ID
    ORDER BY NEWID() 
) LLE1
JOIN p_policy P ON  CC.endorsment_id = P.ID
WHERE CC.ID > 20000000
-- 0 = Claim with Case, 1= Claim with no case
  AND (
       (@isCaseLinkedClaim = 0 AND LLE1.Case_Number IS NOT NULL) 
    OR (@isCaseLinkedClaim = 1 AND LLE1.Case_Number IS NULL)
  )
ORDER BY NEWID(); -- randomize claims
