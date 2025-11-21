/*
LAST UPDATED: Eric Li 2024-06-27 (AUNT-175)
*/
SELECT TOP 40
  c.ID,
  p.EXTERNAL_POLICY_NUMBER,
  (SELECT top 1 cnt.telephone_number FROM cn_contact_telephone cnt
   WHERE cnt.telephone_type = 4 AND cnt.discontinue_date is null AND cnt.contact_id = c.id) as MobilePhone,
  (SELECT top 1 CONCAT(cnt.TELEPHONE_PREFIX, cnt.telephone_number) FROM cn_contact_telephone cnt 
   WHERE cnt.telephone_type = 3 AND cnt.discontinue_date is null AND cnt.contact_id = c.id) as HomePhone,
   FORMAT(ph.policy_start_date,'yyyy-MM-dd HH:mm:ss') as StartDate
FROM p_policy p
  JOIN p_pol_header ph          ON ph.active_policy_id = p.id
  JOIN p_policy_contact ppc     ON ppc.policy_id = PH.ACTIVE_POLICY_ID
  JOIN cn_contact c             ON c.id = ppc.contact_id
  JOIN p_policy_lob ppl         ON PH.ACTIVE_POLICY_ID = ppl.policy_id
WHERE ph.product_id             = 1000000 -- Motor Policy
  AND ph.policy_start_date      between DATEADD(DAY, -27, convert(date, GETDATE())) and convert(date, GETDATE())
  AND ph.status_ID              = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ppc.policy_contact_role   in (6, 8)  -- 6=Policyholder 8=co-Policyholder
  AND C.ENTITY_ID               = 1  -- Individual
  AND p.payment_term_id         = 1000002  -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
  AND p.collection_method_ID    = 4  -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
ORDER BY newid();