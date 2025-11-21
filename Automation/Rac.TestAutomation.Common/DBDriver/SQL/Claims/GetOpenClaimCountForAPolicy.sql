/*
PURPOSE: Return a count of claims against the given policy which:
         * are currently not closed OR
         * have an event date within the past year
AUTHOR:  Jason King
LAST UPDATE: Jason King 2021-07-15
*/
SELECT COUNT(*)
    FROM c_claim C
    JOIN  p_policy p on C.endorsment_id = p.ID
    WHERE (C.CLAIM_STATUS_TYPE_ID != 2 OR 
	       C.CLAIM_EVENT_DATE > DATEADD(year, -1, convert(date, GETDATE())))
    AND p.EXTERNAL_POLICY_NUMBER = @policynumber
;