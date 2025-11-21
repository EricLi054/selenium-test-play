/*
PURPOSE: Find a boat policy that is renewal and has only one bank account
associated with the active policy holder.
This will mean that the member will have the bank account 
for the policy refund determined for them.
LAST UPDATED: Eric Li 2024-06-27 (AUNT-175)
*/
SELECT TOP 40
  p.EXTERNAL_POLICY_NUMBER,
  ppc.contact_id,
  FORMAT(PH.status_renewal_date,'dd/MM/yyyy HH:mm:ss') AS "Renewal Date",
  tbtr.DESCRIPTION, 
  (SELECT top 1 cnt.telephone_number FROM cn_contact_telephone cnt
   WHERE cnt.telephone_type = 4 AND cnt.discontinue_date is null AND cnt.contact_id = ppc.contact_id) as MobilePhone,
  (SELECT top 1 CONCAT(cnt.TELEPHONE_PREFIX, cnt.telephone_number) FROM cn_contact_telephone cnt 
   WHERE cnt.telephone_type = 3 AND cnt.discontinue_date is null AND cnt.contact_id = ppc.contact_id) as HomePhone
FROM p_policy p
  JOIN p_pol_header ph                    ON ph.active_policy_id = p.id
  JOIN p_policy_contact ppc               ON ppc.policy_id = PH.ACTIVE_POLICY_ID
  JOIN p_policy_lob ppl                   ON PH.ACTIVE_POLICY_ID = ppl.policy_id
  JOIN cn_contact c                       ON  c.id = ppc.contact_id
  JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA   ON PPL.ID = PPLTLA.POLICY_LOB_ID 
  JOIN P_POLICY_LOB_ASSET PPLA            ON PPLTLA.LOB_ASSET_ID = PPLA.ID 
  JOIN AS_Asset ass                       ON PPLA.LOB_ASSET_ID = ass.id 
  JOIN AS_MARINE_RACI asm                 ON  ass.ID = asm.ID
  JOIN T_BOAT_TYPE_RACI tbtr              ON  asm.boat_type_id = tbtr.ID
WHERE ph.product_id             = 1000033 -- Boat
  AND ph.status_renewal_date  between DATEADD(day, 1, convert(date, GETDATE())) and DATEADD(day, 14, convert(date, GETDATE()))
  AND (SELECT count(contact_ID) as "Bank Account Count"
     FROM CN_CONTACT_BANK_ACCOUNT ccba
     WHERE ccba.CONTACT_ID = ppc.contact_id
     AND ccba.BANK_NAME not in ('AMEX', 'MASTERCARD', 'VISA')
     AND ccba.discontinue_date is null
     GROUP BY ccba.contact_ID) >= 1
  AND ph.status_ID              = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ppc.policy_contact_role   in (6, 8)  -- 6=Policyholder 8=co-Policyholder
  AND c.ENTITY_ID               = 1  -- Individual
  AND p.payment_term_id         = 4  -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
  AND p.collection_method_ID    = 2  -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
ORDER BY newid();