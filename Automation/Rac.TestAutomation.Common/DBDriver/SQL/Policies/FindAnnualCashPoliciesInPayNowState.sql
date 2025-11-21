/*
PURPOSE: Find Cash Policy that has an outstanding payable instalment (Pending or Submitted).

AUTHOR: Hashili On 02/02/2024
LAST UPDATED: None

*/
SET NOCOUNT ON;
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SELECT TOP 100 
    p.EXTERNAL_POLICY_NUMBER AS POLICY_NUMBER
    ,CASE   WHEN    CONVERT(DATETIME, CONVERT(DATE, P.endors_start_date)) > CONVERT(DATETIME, GETDATE()) --This is to find policies with a Future Dated Endorsement
            AND     CONVERT(DATETIME, CONVERT(DATE, ph.status_renewal_date)) > CONVERT(DATETIME, GETDATE())
            AND     P.endorsment_type_id = 10 --Renewal, This is to find Renewal versions
            THEN    'RENEWAL'
            WHEN    ph.renewal_nr IS NULL --Policy has never been renewed
            AND     ai.STORED_INST_AMOUNT = p.yearly_premium --Outstanding instalment amount matches yearly amount means new business policy is unpaid
            THEN    'NEW_BUSINESS'
            ELSE    'MID_TERM'
     END AS POLICY_STATE
  FROM p_policy p
  JOIN p_pol_header ph              ON  p.id = ph.active_policy_id
  JOIN p_policy_contact ppc         ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
  JOIN cn_contact c                 ON  c.id = ppc.contact_id
  JOIN t_payment_terms tpt          ON  tpt.id = p.payment_term_id
  JOIN AC_INSTALLMENT ai            ON  ph.active_policy_id = ai.POLICY_ID
                                    AND (SELECT count(*) 
                                         FROM   ac_installment ai2
                                         WHERE  ai2.POLICY_ID = ph.active_policy_id
                                         AND    ai2.installment_status IN (1,6) -- 1=Pending, 6=Submitted
                                         AND    ai2.STORED_INST_AMOUNT > 0 AND ai2.STORED_INST_AMOUNT NOT LIKE '-%' -- Cannot pay a negative/refund instalment.
                                         ) = 1 -- PayMyPolicy and MAP Release One only allows payment when there is a single outstanding payable instalment
  LEFT JOIN AC_PAYMENT_MATCH apm    ON  apm.installment_id = ai.id
WHERE 1=1
  AND ai.installment_status IN (@paymentType1, @paymentType2, @paymentType3) -- 1=Pending, 6=Submitted
  AND ai.STORED_INST_AMOUNT > 0 AND ai.STORED_INST_AMOUNT NOT LIKE '-%' -- Cannot pay a negative/refund instalment.
  AND apm.id IS NULL
  AND p.status_ID = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ph.status_ID = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ppc.policy_contact_role IN (6,8) -- 6=Policyholder 8=co-Policyholder
  AND c.ENTITY_ID = 1 --Individual entity type only so no Company/LegalEntity results returned
  AND p.collection_method_ID = 1 -- 1=cash 2=DD 4=CC -- MAP Release Two will allow payment of rejected instalments for CC/DD
  AND ph.product_id = @productid --1000000-Motor,1000001-Home,1000007-ElectricMobility,1000008-Caravan,1000032-MotorCycle,1000033-Boat,4000000-Pet
ORDER BY newid();