/*
PURPOSE: Fetch motor policies that can be cancelled. 
Plus return the primary policy holder.  Based on 
FindPolicyMotorAnyPolicyChangeMyCar.sql

LAST UPDATED: Eric Li 2024-06-27 (AUNT-175)
*/
SELECT TOP 40
  c.ID,
  p.EXTERNAL_POLICY_NUMBER,
  (SELECT top 1 cnt.telephone_number FROM cn_contact_telephone cnt
   WHERE cnt.telephone_type = 4 AND cnt.discontinue_date is null AND cnt.contact_id = c.id) as MobilePhone,
  (SELECT top 1 CONCAT(cnt.TELEPHONE_PREFIX, cnt.telephone_number) FROM cn_contact_telephone cnt 
   WHERE cnt.telephone_type = 3 AND cnt.discontinue_date is null AND cnt.contact_id = c.id) as HomePhone,
   FORMAT(ph.policy_start_date,'yyyy-MM-dd HH:mm:ss') as StartDate,
   ccbar.BSB_NUMBER, 
   ccba.ACCOUNT_NR,
   ccbar.CONTACT_BANK_ACCOUNT_NAME
FROM p_policy p
  JOIN p_pol_header ph          ON ph.active_policy_id = p.id
  JOIN p_policy_contact ppc     ON ppc.policy_id = PH.ACTIVE_POLICY_ID
  JOIN cn_contact c             ON c.id = ppc.contact_id
  JOIN CN_CONTACT_RACI cnr      ON cnr.id = c.id
  JOIN p_policy_lob ppl         ON PH.ACTIVE_POLICY_ID = ppl.policy_id
  JOIN CN_CONTACT_BANK_ACCOUNT cn         ON cn.CONTACT_OWNER_ID = c.id
  JOIN cn_contact_bank_account ccba       ON ccba.id = p.POLICY_OWNER_BANK_ACCOUNT_ID 
  JOIN cn_contact_bank_account_raci ccbar ON ccbar.id = ccba.id
WHERE ph.product_id             = 1000000 -- Motor Policy
  AND ph.policy_end_date        between DATEADD(month, 1, convert(date, GETDATE())) and DATEADD(month, 11, convert(date, GETDATE()))
    AND (SELECT count(contact_ID) as "Bank Account Count"
     FROM CN_CONTACT_BANK_ACCOUNT ccba
     WHERE ccba.CONTACT_ID = ppc.contact_id
     AND ccba.BANK_NAME not in ('AMEX', 'MASTERCARD', 'VISA')
     AND ccba.discontinue_date is null
     GROUP BY ccba.contact_ID) >= 1
  AND ph.status_ID              = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ppc.policy_contact_role   in (6, 8)  -- 6=Policyholder 8=co-Policyholder
  AND C.ENTITY_ID               = 1  -- Individual
  AND (cnr.WESTPAC_CUSTOMER_ID   = c.id or cnr.WESTPAC_CUSTOMER_ID is null) -- avoid contacts with bad tokens
  AND p.payment_term_id         = 1  -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
  AND p.collection_method_ID    = 2  -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card

ORDER BY newid();