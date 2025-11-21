/*
PURPOSE: Find an Annual Cash Home Policy that is due for renewal today.

AUTHOR: Unknown
LAST UPDATED: Troy Hall 2024-01-31
*/
SELECT TOP 40 p.EXTERNAL_POLICY_NUMBER,
    ppc.contact_id,
    p.id,
    cnp.date_of_birth
    ,PH.MAIN_RENEWAL_DATE
    ,p.ENDORSMENT_TYPE_ID as E_ID
    ,FORMAT(p.policy_end_date, 'yyyy-MM-dd hh:mm:ss') AS Term_end
    ,asp.building_year
    ,FORMAT(p.yearly_premium,'0.00') AS Premium
  FROM p_policy p
  JOIN p_pol_header ph          ON p.id = ph.active_policy_id
  JOIN p_policy_contact ppc      ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
  join cn_person cnp             ON cnp.contact_id = ppc.contact_id
  JOIN t_payment_terms tpt      ON tpt.id = p.payment_term_id
  JOIN p_policy_lob ppl                  ON  PH.ACTIVE_POLICY_ID = ppl.policy_id
  JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA  ON  PPL.ID = PPLTLA.POLICY_LOB_ID
  JOIN P_POLICY_LOB_ASSET PPLA           ON  PPLTLA.LOB_ASSET_ID = PPLA.ID
  JOIN AS_Asset ass                      ON  PPLA.LOB_ASSET_ID = ass.id
  JOIN AS_Property asp                   ON  ass.id = asp.ASSET_ID
  JOIN AC_INSTALLMENT ai       ON  ph.active_policy_id = ai.POLICY_ID
  LEFT JOIN AC_PAYMENT_MATCH apm on apm.installment_id = ai.id
WHERE ph.product_id         = 1000001   -- xx0 motor, xx1 home.
  AND (SELECT count(*)
    FROM ac_installment ai2
    WHERE ai2.POLICY_ID = ph.active_policy_id
    AND   ai2.installment_status IN (3,5)  -- 3=Paid, 5=Rejected
    ) = 0
  AND ppc.policy_contact_role in (6,8) -- 6=Policyholder 8=co-Policyholder
  AND apm.id is null
  AND convert(date, ph.status_renewal_date) = convert(date, GETDATE())
  AND PH.MAIN_RENEWAL_DATE    >= convert(date, ph.status_renewal_date)
  AND asp.building_year       > 1900 -- Avoid bug B2C-1270 (Marked "Done" as no interest in fix from PO short term)
  AND p.status_ID             = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ph.status_ID            = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND p.ENDORSMENT_TYPE_ID    = 10 -- 10 Policy Renewal
  AND ph.STATUS_RENEWAL_DATE IS NOT NULL
  AND tpt.description         = 'Yearly'
  AND p.collection_method_ID  = 1 -- 1=cash 2=DD 4=CC
  AND cnp.date_of_birth is not null -- helps screen out company policies.
ORDER BY newid()
;