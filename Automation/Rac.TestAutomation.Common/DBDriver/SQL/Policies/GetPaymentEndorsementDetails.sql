/*
PURPOSE: Get details about a policy that has had its payment details update.

PARAMETERS:
@policyNumber  p_policy.external_policy_number 
@contactId     p_policy_contact.contact_id

AUTHOR: James Barton
LAST UPDATED: James Barton 2022-11-21
*/
SELECT p.external_policy_number
    ,ppc.contact_id 
    ,p.status_id  as policy_status_id 
    ,st.DESCRIPTION as policy_status_description
    ,er.ID as endorsement_identifier 
    ,er.EXTERNAL_CODE as endorsement_external_code
    ,er.DESCRIPTION as endorsement_description 
    ,FORMAT(per.UPDATE_DATE, 'yyyy-MM-dd HH:mm:ss') as update_timestamp
    ,per.update_user
    ,u.name_of_user
    ,ce.email
    ,CASE WHEN policy_certificate_print.event_type = 2000136 THEN 'True' ELSE 'False' END as policy_endorsement_print_event
    ,policy_certificate_print.EVENT_TYPE
    ,policy_certificate_print.EVENT_TYPE_DSC
    ,CASE WHEN policy_event_payment_change.event_type = 2000153 THEN 'True' ELSE 'False' END as policy_event_payment_change
    ,policy_event_payment_change.EVENT_TYPE
    ,policy_event_payment_change.EVENT_TYPE_DSC
    ,CASE WHEN debit_admendment.ENDORSMENT_REASON_ID = 1000006 THEN 'True' ELSE 'False' END as debit_admendment
    ,debit_admendment.ENDORSMENT_REASON_ID
    ,debit_admendment.ENDORSEMENT_DESCRIPTION
    ,event_records.EVENT_COUNT
FROM p_policy p
    JOIN p_pol_header ph                         ON ph.active_policy_id = p.id
    JOIN P_POLICY_ENDORSMENT_REASON per          ON per.policy_id = p.id
    JOIN T_ENDORSMENT_REASON er                  ON er.id = per.ENDORSMENT_REASON_ID 
    JOIN T_STATUS_CODE st                        ON st.id = p.STATUS_ID
    JOIN T_USER u                                ON u.USERID = per.update_user
    JOIN p_policy_contact ppc                    ON ppc.policy_id = PH.ACTIVE_POLICY_ID
    JOIN CN_CONTACT_EMAIL ce                     ON ce.CONTACT_ID = ppc.CONTACT_ID
    LEFT JOIN (SELECT  e.policy, e.event_type,tet.EVENT_TYPE_DSC
               FROM    e_event e
               JOIN    T_EVENT_TYPE tet ON tet.EVENT_TYPE = e.EVENT_TYPE
               WHERE   e.policy is NOT NULL
               AND     e.event_type = 2000136)  -- Policy Endorsement Certificate Print
                policy_certificate_print        ON policy_certificate_print.policy = p.id
     LEFT JOIN (SELECT  e.policy, e.event_type,tet.EVENT_TYPE_DSC
               FROM    e_event e
               JOIN    T_EVENT_TYPE tet ON tet.EVENT_TYPE = e.EVENT_TYPE
               WHERE   e.policy is NOT NULL
               AND     e.event_type = 2000153)  -- Policy Endorsement - Payment Plan Change
                policy_event_payment_change        ON policy_event_payment_change.policy = p.id
     LEFT JOIN (SELECT  per.POLICY_ID, per.ENDORSMENT_REASON_ID, ter.DESCRIPTION as "ENDORSEMENT_DESCRIPTION"
               FROM    P_POLICY_ENDORSMENT_REASON per
               JOIN    T_ENDORSMENT_REASON ter ON ter.ID = per.ENDORSMENT_REASON_ID
               WHERE   per.POLICY_ID is NOT NULL
               AND     per.ENDORSMENT_REASON_ID = 1000006)  -- Amend instalment debit details
               debit_admendment       ON debit_admendment.policy_ID = p.id
     JOIN      (SELECT p2.POLICY_HEADER_ID, count(*)as "EVENT_COUNT"
               FROM p_policy p2
               JOIN p_pol_header ph2  ON ph2.id = p2.POLICY_HEADER_ID 
	                                  AND p2.UPDATE_DATE > convert(datetime, DATEADD(minute, -10, GETDATE())) 
               GROUP BY p2.POLICY_HEADER_ID) event_records on event_records.POLICY_HEADER_ID = p.POLICY_HEADER_ID
WHERE p.external_policy_number like @policyNumber
AND ppc.contact_id = @contactId
AND ce.discontinue_date is NULL
AND ce.EMAIL_TYPE = 1 -- Only private emails are updated by B2C PCM / SPARK