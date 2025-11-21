/*
PURPOSE: Return the claim's type, scenario and event datetime.
         Also returns the police report reference and date 
         if they were provided in the claim.
AUTHOR:  unknown
LAST UPDATE: Troy Hall 2024-07-11
*/
SELECT  cl.claim_cause_of_loss_id, 
        tgc.DESCRIPTION as GENERAL_CLAIM_TYPE,
        cc.CLAIM_SCENARIO_ID,
        CASE 
            WHEN tcs.DESCRIPTION = 'Damage' 
                THEN tcs.EXTERNAL_CODE 
            ELSE tcs.DESCRIPTION 
        END SCENARIO_DESC,
        police.POLICE_REFERENCE_NUMBER,
        format(police.REPORT_DATE, 'yyyy-MM-dd') AS POLICE_REPORT_DATE,
        format(cl.CLAIM_EVENT_DATE, 'yyyy-MM-dd HH:mm') AS EVENT_DATE_AND_TIME
FROM    C_claim cl      
        JOIN c_claim_raci cc ON cl.id = cc.id
        JOIN T_GENERAL_CLAIM_TYPE tgc ON cl.claim_cause_of_loss_id = tgc.id
        JOIN T_CLAIM_SCENARIOS_RACI tcs ON cc.CLAIM_SCENARIO_ID = tcs.id
        LEFT JOIN C_CLAIM_POLICE_REPORT police ON police.CLAIM_ID = cl.ID
WHERE   cl.claim_number = @claimNumber
;