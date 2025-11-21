/*
PURPOSE: Fetch status for Storm Claim agenda steps
AUTHOR: Soumick Dey
LAST UPDATED: Troy Hall 2024-05-03
*/
SELECT  DISTINCT(tas.DESCRIPTION) as Agenda_step
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
WHERE   CT.CLAIMANT_SIDE_id = 1 --Claimant Side: 1 Policyholder, 2 Third Pary, 3 Insured Claimant.
  AND   cc.ROLE_ID          = 1 --Claim Contact role: 1 Claimant, 2 Witness, 6 PolicyHolder, 9 Driver, 1000017 Third Party
  AND   cl.claim_number     = @ClaimNumber
  AND	tas.id in ('3000000','3000002','3000001','3000006','3000007','3000010','3000011'); --Agenda Step id: 3000000 Claim Lodged, 3000001 Building - Quote Authorised, 3000002 Contents - Quote Authorised, 3000006  Building - Repairs Complete, 3000007 Contents - Items Replaced or Paid Out, 3000011 Payments Settled


 