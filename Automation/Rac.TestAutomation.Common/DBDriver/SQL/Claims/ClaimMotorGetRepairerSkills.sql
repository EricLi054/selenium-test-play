SELECT  /*+ALL_ROWS*/ 
        c.ID,
        c.EXTERNAL_CONTACT_NUMBER,
        c.name as RepairerName,
        tspa.description as service_area,
        cspa.service_provider_area_id as service_area_value,
        case when cspr.can_readyDrive = 1 then '1' 
		    else '0' end can_readyDrive,
        case when cspr.hire_car_flag = 1 then '1' 
		    else '0' end hire_car_flag,
        (SELECT count(0) FROM cn_contact_role ccr JOIN T_CONTACT_ROLE tcr ON tcr.ID = ccr.ROLE_ID 
          WHERE ccr.CONTACT_ID = c.id AND tcr.EXTERNAL_CODE = 'AutoAuthRepairer') AS AutoAuthoriseRole
FROM          C_claim cl                        
        JOIN  C_CLAIM_CONTACT CC              ON  Cl.ID = CC.CLAIM_ID
        JOIN  C_CLAIMANT CT                   ON  CC.RELATED_ENTITY_ID = CT.ID
        JOIN  cn_contact c                    ON  cc.contact_id = c.id
    LEFT JOIN   cn_service_provider csp ON  cc.contact_id = csp.contact_id
    LEFT JOIN   cn_service_provider_raci cspr ON cspr.id = csp.id
    LEFT JOIN   cn_service_provider_areas cspa ON cspa.service_provider_id = csp.id
    LEFT JOIN   cn_contact_role ccr     ON  cc.contact_id = ccr.contact_id
    LEFT JOIN   t_service_provider_area tspa    ON  tspa.id = cspa.service_provider_area_id
    LEFT JOIN   cn_referral_plan crp        ON  csp.ID = crp.service_provider_id
WHERE   CT.CLAIMANT_SIDE_id = 1 --1 Policyholder, 2 Third Pary, 3 Insured Claimant
  AND   cc.ROLE_ID = 5 --5 Repairer --9 Driver, --2 Witness
  AND   cl.claim_number = @claimNumber 
  and   csp.service_type_id = 2 
  and   crp.year =  format(getdate(),'yyyy')
  and   crp.month = format(getdate(),'MM')
;