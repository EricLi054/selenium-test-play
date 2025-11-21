/*
PURPOSE: Return a count of claims against the given policy which:
         * have damage type Storm & Tempest Damage to Fence Only
         * have an event date within the past year
AUTHOR: Soumick Dey
LAST UPDATE: Troy Hall 2023-11-07
*/
SELECT  COUNT(*)
FROM            c_claim C
        JOIN    c_claim_raci cr                 ON  C.ID = cr.ID
        JOIN    c_claim_contact ccc             ON  C.ID = ccc.claim_id
        JOIN    c_claimant cc                   ON  ccc.ID = cc.ID
        JOIN    c_claimant_asset cca            ON  cc.ID = cca.claimant_id
        JOIN    c_damage cd                     ON  cca.ID = cd.claim_asset_id
        JOIN    p_policy P                      ON  C.endorsment_id = P.ID
        JOIN    p_pol_header ph                 ON  ph.ID = P.policy_header_id   
        JOIN    t_damage_code tdc               ON  cd.damage_code_id = tdc.ID
WHERE p.external_policy_number = @policynumber
AND tdc.DESCRIPTION = 'Storm & Tempest Damage to Fence Only'
AND C.CLAIM_EVENT_DATE > DATEADD(year, -1, convert(date, GETDATE()))
;