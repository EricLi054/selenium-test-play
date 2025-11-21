SELECT  tdc.description AS DAMAGE_CODE
        ,tas.DESCRIPTION as Agenda_step
        ,astat.DESCRIPTION as Agenda_Status
FROM          C_claim cl                        
        JOIN  C_CLAIM_CONTACT CC              ON  Cl.ID = CC.CLAIM_ID
        JOIN  C_CLAIMANT CT                   ON  CC.ID = CT.ID 
        JOIN  C_CLAIMANT_ASSET CCA            ON  CT.ID = CCA.CLAIMANT_ID
        JOIN  C_DAMAGE CD                     ON  CCA.ID = CD.CLAIM_ASSET_ID
        JOIN  C_DAMAGED_OBJECT_AGENDA_STEP OA ON  oa.ENTITY_ID = cca.id and OA.ENTITY_NUMBER = 405
        JOIN  T_DAMAGE_CODE TDC               ON  CD.DAMAGE_CODE_ID = TDC.ID
        JOIN  T_AGENDA_STEP_GROUP tasg        ON  tasg.id = OA.GROUP_STEP_ID
        JOIN  T_AGENDA_STEP tas               ON  tas.id = tasg.AGENDA_STEP
        JOIN  T_AGENDA_STATUS astat           ON  astat.id = OA.STEP_STATUS_ID
WHERE   CT.CLAIMANT_SIDE_id = 1 --Policyholder
AND     cc.ROLE_ID = 1 --Claimant
AND     cl.claim_number = @claimNumber
AND     tas.id = @agendaStep
;