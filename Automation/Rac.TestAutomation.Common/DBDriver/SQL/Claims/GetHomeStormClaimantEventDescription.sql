/*
PURPOSE: Return the contents of the Claimant Devent Description for this claim to 
		 evaluate against our expected results.
         The values for claim_number and claimant_side are returned to assist with 
         debugging, I don't expect to make use of them in the code.
AUTHOR: Troy Hall
LAST UPDATE: Troy Hall 2024-30-02
*/
SELECT
    c.claim_number,
    ccl.CLAIMANT_EVENT_DESCRIPTION,
    tcs.description AS claimant_side
FROM
     c_claim c
JOIN c_claim_contact ccn ON  C.ID = ccn.claim_id
JOIN c_claimant ccl      ON  ccn.ID = ccl.ID
JOIN t_claimant_side tcs ON  tcs.id = ccl.claimant_side_id
WHERE
    c.id = @claimNumber;