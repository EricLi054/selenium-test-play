/*
PURPOSE: This query will return a positive count if the policy has an instalment 
		collection date matching the current date, thus disqualifying it as a 
		candidate.

AUTHOR: Troy Hall
LAST UPDATE: Troy Hall 2023-10-23
*/
SELECT 
count(p.EXTERNAL_POLICY_NUMBER) as InstalmentDueToday
    /* The following lines may be useful for debugging but are not required
	,FORMAT(ai.COLLECTION_DATE, 'd MMM yyyy') AS NextPayDue
    ,FORMAT(ai.estimated_amount,'0.00') AS InstAmt
    ,tist.DESCRIPTION as InstStatus
    ,case when ai.INSTALLMENT_TYPE = 1 then 'true' else 'false' end isRecurring
	--*/
  FROM p_policy p
	  JOIN ac_installment ai        ON p.id = ai.POLICY_ID
	  JOIN T_INSTALLMENT_STATUS_TYPE tist ON tist.id = ai.INSTALLMENT_STATUS
  WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber
	  AND p.external_policy_number in 
	  (
		SELECT p1.EXTERNAL_POLICY_NUMBER
		FROM p_policy p1
		  JOIN ac_installment ai1        ON p1.id = ai1.POLICY_ID
		  JOIN T_INSTALLMENT_STATUS_TYPE tist1 ON tist1.id = ai1.INSTALLMENT_STATUS
		WHERE p1.EXTERNAL_POLICY_NUMBER = p.external_policy_number
		  and CAST(ai1.COLLECTION_DATE as DATE) = CAST(getdate() as DATE)
	  )
;