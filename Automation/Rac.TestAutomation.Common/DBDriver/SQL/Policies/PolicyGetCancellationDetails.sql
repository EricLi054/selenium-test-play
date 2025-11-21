/*
PURPOSE: Get details about a cancelled policy and the initiator

AUTHOR: James Barton
LAST UPDATED: James Barton 2022-12-30
*/
SELECT p.external_policy_number
    ,ppc.contact_id 
    ,p.status_id  as policy_status_id 
    ,st.DESCRIPTION as policy_status_description
    ,er.ID as endorsement_identifier 
    ,er.EXTERNAL_CODE as endorsement_external_code
    ,er.DESCRIPTION as endorsement_description 
    ,p.premium_for_coll 
    ,CASE WHEN ai.ESTIMATED_AMOUNT is NULL THEN '0.00' ELSE FORMAT(ai.ESTIMATED_AMOUNT,'0.00') END as refund_amount
    ,FORMAT(per.UPDATE_DATE, 'yyyy-MM-dd HH:mm:ss') as update_timestamp
    ,per.update_user
    ,u.name_of_user
    ,FORMAT(ph.cancel_from,'yyyy-MM-dd HH:mm:ss') as cancel_from 
    ,ci.id as cancellation_initiator_id
    ,ci.description as cancellation_initiator_description
    ,cm.id as cancel_method_id
    ,cm.dsc as cancel_method_description
    ,cm.cancellation_fee
    ,ce.email
    ,CASE WHEN policy_cancellation_print.event_type = 2000078 THEN 'True' ELSE 'False' END as cancellation_print_event
FROM p_policy p
    JOIN p_pol_header ph                         ON ph.active_policy_id = p.id
    JOIN P_POLICY_ENDORSMENT_REASON per          ON per.policy_id = p.id
    JOIN T_ENDORSMENT_REASON er                  ON er.id = per.ENDORSMENT_REASON_ID 
    JOIN T_STATUS_CODE st                        ON st.id = p.STATUS_ID
    JOIN T_USER u                                ON u.USERID = per.update_user
    JOIN T_CANCEL_INITIATOR ci                   ON ci.id = p.CANCEL_INITIATOR_ID
    JOIN T_CANCELATION_METHOD cm                 ON cm.id = p.CANCEL_METHOD_ID
    JOIN p_policy_contact ppc                    ON ppc.policy_id = PH.ACTIVE_POLICY_ID
    JOIN CN_CONTACT_EMAIL ce                     ON ce.CONTACT_ID = ppc.CONTACT_ID
    LEFT JOIN ac_installment ai                  ON p.id = ai.POLICY_ID AND ai.INSTALLMENT_STATUS in (1,7) -- Pending or Booked
    LEFT JOIN (SELECT  e.policy, e.event_type
               FROM    e_event e
               WHERE   e.policy is NOT NULL
               AND     e.event_type = 2000078)
                policy_cancellation_print        ON policy_cancellation_print.policy = p.id
WHERE p.external_policy_number like @policynumber
AND ppc.contact_id = @ContactId
AND ce.discontinue_date is NULL
AND ce.EMAIL_TYPE = 1 -- Only private emails are updated by B2C PCM / SPARK
;
