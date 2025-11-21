/*
PURPOSE: Return Open policy holder claim numbers for Storm & Tempest Damage to Fence Only within the last 
         month. These claims mustnot have any existing Authorised Quote Reserve or active Payment Block 
         irregularities.

AUTHOR:  Soumick Dey
LAST UPDATE: Troy Hall 2024-09-26
*/
SELECT  TOP 100
        C.claim_number, ccc.contact_id
        /* Comment this line to enable return of more detail if debugging query
        ,tci.REQUIRES_PAYMENTS_BLOCK
        ,tci.DESCRIPTION AS IRREGULARITY_DESCRIPTION 
        ,tcsr.description as claim_scenario_description--*/
FROM            c_claim C
        JOIN    c_claim_raci cr                 ON  C.ID = cr.ID
        JOIN    c_claim_contact ccc             ON  C.ID = ccc.claim_id
        JOIN    c_claimant cc                   ON  ccc.ID = cc.ID
        JOIN    c_claimant_asset cca            ON  cc.ID = cca.claimant_id
        JOIN    c_damage cd                     ON  cca.ID = cd.claim_asset_id
        JOIN    p_policy P                      ON  C.endorsment_id = P.ID
        JOIN    p_pol_header ph                 ON  ph.ID = P.policy_header_id   
        JOIN    t_damage_code tdc               ON  cd.damage_code_id = tdc.ID
        JOIN    C_CLAIM_IRREGULARITY cir        ON  cir.entity_id = c.claim_number
        JOIN    T_CLAIM_IRREGULARITY tci        ON  tci.id = cir.irregularity_id
        JOIN    T_CLAIM_SCENARIOS_RACI tcsr     ON  tcsr.id = cr.claim_scenario_id
AND     cc.claimant_side_id = 1 --Policyholder Claimant Side
AND     C.claim_status_type_id != 2 --2 = Closed
AND     C.claim_event_date > DATEADD(month, -1, convert(date, GETDATE())) -- Within the past month
AND     ph.product_id = 1000001 -- Home claims
AND     tdc.DESCRIPTION = 'Storm & Tempest Damage to Fence Only' --Description of damage code
        -- Should not already have a Authorised Quote reserve on it.
AND     (SELECT count(*) from C_RESERVE cres where cres.CLAIM_ID = C.ID and cres.RESERVE_REASON_ID = 1000002) = 0
        -- Protect from bad or incomplete claims by requiring scenario
AND     cr.CLAIM_SCENARIO_ID is not null
        -- Claim has no active PAYMENT BLOCK irregularities 
and     c.claim_number not in (
                                select cci2.entity_id as claim_number
                                from 
                                C_CLAIM_IRREGULARITY cci2 
                                join T_CLAIM_IRREGULARITY tci2 on cci2.IRREGULARITY_ID = tci2.id 
                                where 
                                cci2.entity_id = c.claim_number 
                                and cci2.discontinue_date is null
                                and tci2.REQUIRES_PAYMENTS_BLOCK = 1
                              )
ORDER BY newid()
;