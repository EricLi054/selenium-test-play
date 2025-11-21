/*
PURPOSE: This query fetches all policy instalments (Annual/Monthly) of the policy
         from today onwards.

AUTHOR: Jason King
LAST UPDATE: Kris Raymundo 2021-08-09
*/
SELECT p.EXTERNAL_POLICY_NUMBER
    ,FORMAT(ai.COLLECTION_DATE, 'd MMM yyyy') AS NextPayDue
    ,FORMAT(ai.estimated_amount,'0.00') AS InstAmt
    ,tist.DESCRIPTION
    ,case when ai.INSTALLMENT_TYPE = 1 then 'true' else 'false' end isRecurring
  FROM p_policy p
  JOIN ac_installment ai        ON p.id = ai.POLICY_ID
  JOIN T_INSTALLMENT_STATUS_TYPE tist ON tist.id = ai.INSTALLMENT_STATUS
  WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber
  AND ai.UPDATE_DATE > convert(datetime, DATEADD(minute, -10, GETDATE()))
  and ai.COLLECTION_DATE > convert(datetime, DATEADD(day, -1, GETDATE()))
  ORDER BY ai.UPDATE_DATE DESC;