/*
PURPOSE: Find the next pending instalment (date and amount) for the supplied policy. 

AUTHOR: James Barton
LAST UPDATE: James Barton 2022-11-11
*/
SELECT p.EXTERNAL_POLICY_NUMBER,
  (SELECT top 1 ai.ESTIMATED_AMOUNT
   FROM    ac_installment ai
   JOIN    p_policy p2 on p2.id = ai.POLICY_ID
   WHERE   p2.EXTERNAL_POLICY_NUMBER = p.EXTERNAL_POLICY_NUMBER 
   and ai.INSTALLMENT_STATUS = 1 -- PENDING status
   order by ai.INSTALLMENT_NUMBER asc) as AMOUNT_DUE,
  (SELECT top 1 format(ai.COLLECTION_DATE,'dd/MM/yyyy')
   FROM    ac_installment ai
   JOIN    p_policy p2 on p2.id = ai.POLICY_ID
   WHERE   p2.EXTERNAL_POLICY_NUMBER = p.EXTERNAL_POLICY_NUMBER 
   and ai.INSTALLMENT_STATUS = 1 -- PENDING status
   order by ai.INSTALLMENT_NUMBER asc) as COLLECTION_DATE,
  (SELECT top 1 format(ai.ORIGINAL_COLLECTION_DATE,'dd/MM/yyyy')
   FROM    ac_installment ai
   JOIN    p_policy p2 on p2.id = ai.POLICY_ID
   WHERE   p2.EXTERNAL_POLICY_NUMBER = p.EXTERNAL_POLICY_NUMBER 
   and ai.INSTALLMENT_STATUS = 1 -- PENDING status
   order by ai.INSTALLMENT_NUMBER asc) as ORIGINAL_COLLECTION_DATE,
   ccba.type as AccountType,
   ccbar.BSB_NUMBER as "BsbNumber", 
   ccba.ACCOUNT_NR as "AccountNumber",
   ccbar.CONTACT_BANK_ACCOUNT_NAME as "BankAccountName",
   ccba.BANK_NAME as "CreditCardIssuer",  -- Gives credit card issuer
   ccba.CREDIT_CARD_HOLDER_NAME as "CreditCardAccountName",
   format(ccba.VALID_DATE,'dd/MM/yyyy') as EXPIRY_DATE
FROM p_policy p
  JOIN p_pol_header ph                    ON ph.active_policy_id = p.id
  JOIN cn_contact_bank_account ccba       ON ccba.id = p.POLICY_OWNER_BANK_ACCOUNT_ID 
  JOIN cn_contact_bank_account_raci ccbar ON ccbar.id = ccba.id
WHERE p.EXTERNAL_POLICY_NUMBER = @policyNumber
