/*
PURPOSE: Get the next installment value after renewal endorsement.

AUTHOR: Unknown
LAST UPDATED: Jason King 2020-09-10
*/
SELECT p.EXTERNAL_POLICY_NUMBER,
       FORMAT(sum(ai.ESTIMATED_AMOUNT),'0.00') as expected_prem
  FROM p_policy p 
  JOIN AC_INSTALLMENT ai        ON p.id = ai.POLICY_ID
  WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber
  AND ai.collection_date >= convert(date, GETDATE())
GROUP BY p.EXTERNAL_POLICY_NUMBER;