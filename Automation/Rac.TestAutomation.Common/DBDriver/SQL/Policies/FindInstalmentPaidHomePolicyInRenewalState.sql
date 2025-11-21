/*
PURPOSE: Find a Monthly Home Policy that is in renewal state

AUTHOR: Unknown
LAST UPDATED: Troy Hall 2024-01-31
*/
SELECT TOP 40 p.EXTERNAL_POLICY_NUMBER,
    ppc.contact_id,
    p.id,
    cnp.date_of_birth,
    FORMAT(PH.MAIN_RENEWAL_DATE,'d MMMM yyyy'),
    p.ENDORSMENT_TYPE_ID as E_ID
    ,FORMAT(p.policy_end_date, 'yyyy-MM-dd hh:mm:ss') AS Term_end
    ,asp.building_year
    ,FORMAT(p.yearly_premium,'N2') AS Premium
    ,REPLACE(FORMAT(ai.COLLECTION_DATE, 'd MMMM yyyy'),'   ',' ') AS NextPayDueL
    ,FORMAT(ai.COLLECTION_DATE, 'd MMM yyyy') AS NextPayDueS
    ,FORMAT(ai.estimated_amount,'0.00') AS InstAmt
  FROM p_policy p
  JOIN p_pol_header ph          ON p.id = ph.active_policy_id
  JOIN p_policy_contact ppc      ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
  JOIN cn_person cnp             ON cnp.contact_id = ppc.contact_id
  JOIN t_collection_method tcm  ON tcm.id = p.COLLECTION_METHOD_ID
  JOIN t_payment_terms tpt      ON tpt.id = p.payment_term_id
  JOIN ac_installment ai        ON ph.active_policy_id = ai.POLICY_ID
  JOIN  p_policy_lob ppl                  ON  PH.ACTIVE_POLICY_ID = ppl.policy_id
  JOIN  P_POLICY_LOB_TO_LOB_ASSET PPLTLA  ON  PPL.ID = PPLTLA.POLICY_LOB_ID
  JOIN  P_POLICY_LOB_ASSET PPLA           ON  PPLTLA.LOB_ASSET_ID = PPLA.ID
  JOIN  P_POLICY_LOB_ASSET_RACI PLAR      ON  ppla.ID = plar.id
  JOIN  AS_Asset ass                      ON  PPLA.LOB_ASSET_ID = ass.id
  JOIN  AS_Property asp                   ON  ass.id = asp.ASSET_ID
  WHERE ph.product_id           = 1000001 -- xx0 motor, xx1 home.
  AND (SELECT count(*)
    FROM ac_installment ai2
    WHERE ai2.POLICY_ID = ph.active_policy_id
    AND   ai2.installment_status IN (3,5)  -- 3=Paid, 5=Rejected
    ) = 0
  AND ppc.policy_contact_role in (6,8) -- 6=Policyholder 8=co-Policyholder
  AND ph.status_renewal_date between DATEADD(day, 1, convert(date, GETDATE())) and DATEADD(day, 25, convert(date, GETDATE())) 
  AND PH.MAIN_RENEWAL_DATE    > ph.status_renewal_date
  AND ai.COLLECTION_DATE between GETDATE() and DATEADD(day, 28, convert(date, GETDATE())) 
  AND ai.INSTALLMENT_STATUS = 1      -- 1=Pending
  AND asp.building_year       > 1900 -- Avoid bug B2C-1270 (Marked "Done" as no interest in fix from PO short term)
  AND p.status_ID             = 20   -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ph.status_ID            = 20   -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND p.ENDORSMENT_TYPE_ID    = 10   -- 10 Policy Renewal
  AND ph.STATUS_RENEWAL_DATE IS NOT NULL
  AND p.payment_term_id       = 4 -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
  AND p.collection_method_ID  = 2 -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
  AND cnp.date_of_birth is not null -- helps screen out company policies.
ORDER BY newid()
;