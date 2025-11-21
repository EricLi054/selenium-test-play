SELECT TOP 50 C.claim_number
FROM            c_claim C
        JOIN    c_claim_raci cr                 ON  C.ID = cr.ID
        JOIN    c_claim_contact ccc             ON  C.ID = ccc.claim_id
        JOIN    c_claimant cc                   ON  ccc.ID = cc.ID
        JOIN    c_claimant_asset cca            ON  cc.ID = cca.claimant_id
        JOIN    c_damage cd                     ON  cca.ID = cd.claim_asset_id
   LEFT JOIN    c_claim_group ccg               ON  C.claim_group = ccg.ID
        JOIN    p_policy P                      ON  C.endorsment_id = P.ID
        JOIN    p_pol_header ph                 ON  ph.ID = P.policy_header_id
   LEFT JOIN    t_claim_scenarios_raci tcsr     ON  cr.claim_scenario_id = tcsr.ID   
WHERE   1=1
AND     cc.claimant_side_id = 1  --Policyholder Claimant Side
AND     C.claim_status_type_id != 2 --2 = Closed
AND     FORMAT(C.insert_date, 'yyyy-MM-dd') >= DATEADD(day, -90, convert(date, GETDATE())) --In Last 90 Days
AND     ph.product_id = 1000000 --Motor
AND     tcsr.DESCRIPTION = 'Glass Damage' -- Damage Type Only Motor Glass
ORDER BY NEWID();