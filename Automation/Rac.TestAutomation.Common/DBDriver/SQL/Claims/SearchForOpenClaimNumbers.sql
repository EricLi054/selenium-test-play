/*
PURPOSE: Fetch claim not in closed state from all products

AUTHOR: Hashili
LAST UPDATED: 2025-02-05
*/
SELECT TOP 2 C.claim_number, P.EXTERNAL_POLICY_NUMBER,LLE.LITIGATION_ID
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
   LEFT JOIN    L_LITIGATION_ENTITY LLE         ON  LLE.ENTITY_ID = C.ID
WHERE   1=1
AND     cc.claimant_side_id = 1  --Policyholder Claimant Side
AND     C.claim_status_type_id != 2 --2 = Closed
AND     FORMAT(C.insert_date, 'yyyy-MM-dd') >= DATEADD(day, -90, convert(date, GETDATE())) --In Last 90 Days
AND     ((@AnyProductType = '1') OR (ph.product_id = @productType))
AND     c.CLAIM_NUMBER > 20000000 -- CCI catalogue works only for claim number starts with 2 on NPE
ORDER BY NEWID();
