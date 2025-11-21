/*
PURPOSE: Find credit card, amount and date for an upcoming instalment on a policy.
The policy needs be active and mid term with only paid and pending instalments.

Additionally phone numbers are included, as badly formed phone numbers can 
cause updates to the Insurance Contact Service to fail.

AUTHOR: James Barton
LAST UPDATE:28/07/2023
*/
SELECT TOP 40
   p.EXTERNAL_POLICY_NUMBER as PolicyNumber,
   c.ID                     as ContactID,
  (SELECT top 1 cnt.telephone_number FROM cn_contact_telephone cnt
    WHERE cnt.telephone_type = 4 AND cnt.discontinue_date is null AND cnt.contact_id = c.id) as MobilePhone,
  (SELECT top 1 CONCAT(cnt.TELEPHONE_PREFIX, cnt.telephone_number) FROM cn_contact_telephone cnt 
   WHERE cnt.telephone_type = 3 AND cnt.discontinue_date is null AND cnt.contact_id = c.id) as HomePhone
FROM p_policy p
  JOIN p_pol_header ph                    ON ph.active_policy_id = p.id
  JOIN p_policy_contact ppc               ON ppc.policy_id = PH.ACTIVE_POLICY_ID
  JOIN cn_contact c                       ON c.id = ppc.contact_id
  JOIN CN_CONTACT_RACI cnr                ON cnr.id = c.id
  JOIN p_policy_lob ppl                   ON PH.ACTIVE_POLICY_ID = ppl.policy_id
  JOIN cn_contact_bank_account ccba       ON ccba.id = p.POLICY_OWNER_BANK_ACCOUNT_ID 
WHERE ph.product_id             = @productId -- 1000000:  Motor Policy
  AND ph.policy_end_date        between DATEADD(day, 32, convert(date, GETDATE())) and DATEADD(day, 336, convert(date, GETDATE()))
  AND (SELECT count(*) FROM CN_CONTACT_BANK_ACCOUNT ccba2 WHERE ccba2.CONTACT_ID = ccba.CONTACT_ID
       AND ccba.BANK_NAME in ('AMEX', 'MASTERCARD', 'VISA') and ccba.DISCONTINUE_DATE is null) >= 1
  AND ph.status_ID              = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ppc.policy_contact_role   = 6  -- 6=Policyholder 8=co-Policyholder
  AND C.ENTITY_ID               = 1  -- Individual
  AND cnr.WESTPAC_CUSTOMER_ID   = c.id -- Correctly tokenised with Westpac
  AND p.collection_method_ID    = 4 -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
  AND p.payment_term_id         = 1000002      -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
  AND (SELECT count(ai2.id) as "Not Pending or Paid Count"
       FROM AC_INSTALLMENT ai2
       WHERE ai2.POLICY_ID = ph.ACTIVE_POLICY_ID
       AND ai2.INSTALLMENT_STATUS NOT in (1,3)) = 0 
ORDER BY newid();
