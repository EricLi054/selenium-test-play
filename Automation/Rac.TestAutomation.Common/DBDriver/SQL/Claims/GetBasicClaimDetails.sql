/*
PURPOSE: Fetch basic claim details.

AUTHOR: Unknown
LAST UPDATED: Jason King 2021-09-14
*/
SELECT  cl.claim_number AS CLAIM_NUMBER
        ,p.external_policy_number as Policy_Number
        ,FORMAT(CL.CLAIM_EVENT_DATE, 'yyyy-MM-dd hh:mm:ss') AS EVENT_DATE
        ,tdc.description AS DAMAGE_CODE
        ,tcst.description AS CLAIM_STATUS
        ,tds.description as DAMAGE_STATUS
        ,tcs.description as Claimant_Status
        ,FORMAT((select crl.amount from c_reserve_line crl where crl.RESERVE_ID = cr.ID), '0') as ReserveAmount
        ,crr.DESCRIPTION as reserve_reason
        ,jdr.job_number as ARNIE_Number
        ,cl.CLAIM_EVENT_DESCRIPTION
        ,ctr.CALCULATED_EXCESS
FROM          C_claim cl                        
        JOIN  C_CLAIM_CONTACT CC              ON  Cl.ID = CC.CLAIM_ID
        JOIN  C_CLAIMANT CT                   ON  CC.ID = CT.ID 
        JOIN  C_CLAIMANT_RACI CTR             ON  CT.ID = CTR.ID 
        JOIN  C_CLAIMANT_ASSET CCA            ON  CT.ID = CCA.CLAIMANT_ID
        JOIN  C_DAMAGE CD                     ON  CCA.ID = CD.CLAIM_ASSET_ID
        JOIN  C_RESERVE cr                    ON  cd.id = cr.DAMAGE_ID
        LEFT OUTER JOIN C_ARNIE_JOB_DETAILS_RACI jdr ON cca.id = jdr.claimant_asset_id
        JOIN  p_policy p                      ON  cl.endorsment_id = p.id
        JOIN  T_CLAIM_STATUS_TYPE tcst        ON  cl.CLAIM_STATUS_TYPE_ID = tcst.id
        JOIN  T_Claimant_status tcs           ON  ct.CLAIMANT_STATUS_ID = tcs.id
        JOIN  T_DAMAGE_CODE TDC               ON  CD.DAMAGE_CODE_ID = TDC.ID
        JOIN  T_DAMAGE_STATUS tds             ON  cd.DAMAGE_STATUS_ID = tds.id
        JOIN  T_CLAIM_RESERVE_REASON crr      ON  cr.RESERVE_REASON_ID = crr.id
WHERE   CT.CLAIMANT_SIDE_id = 1 --Policyholder
AND     cc.ROLE_ID = 1 --Claimant
AND     cl.claim_number = @ClaimNumber
ORDER BY tdc.description DESC
;