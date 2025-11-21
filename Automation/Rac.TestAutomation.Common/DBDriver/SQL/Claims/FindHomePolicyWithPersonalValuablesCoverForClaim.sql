/*
PURPOSE: Return a list of policies only for Outside Personal Valuables Specified (OVS) cover
AUTHOR:  Eric Li 2024-07-15

*/
SELECT TOP 20
    cn.id                                     ,
    p.external_policy_number                  AS PolicyNumber,
	ppc.CONTACT_ID,
    tplot_hcn.external_code                   AS externalCode, 
    c.city_name                               AS CityName,     
    c.zip                                     AS Zip        
FROM p_policy p
    JOIN p_pol_header ph      ON ph.active_policy_id = p.id
    JOIN p_policy_contact ppc ON ppc.policy_id = ph.active_policy_id
    JOIN t_product tp         ON tp.product_id = ph.product_id
    JOIN p_policy_lob ppl                    ON ph.active_policy_id = ppl.policy_id
    JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA    ON ppl.id = PPLTLA.policy_lob_id
    JOIN P_POLICY_LOB_ASSET PPLA             ON PPLA.id = PPLTLA.lob_asset_id
    JOIN AS_Asset ass                        ON PPLA.lob_asset_id = ass.id
    JOIN AS_Property asp                     ON ass.id = asp.asset_id
    JOIN cn_contact cn                       ON ppc.contact_id = cn.id 
    JOIN cn_person cp                        ON cp.contact_id = cn.id
    JOIN cn_contact_telephone cn_tel         ON cn_tel.contact_id = cn.id 
                                              AND cn_tel.discontinue_date IS NULL -- active
                                              AND cn_tel.telephone_type = 4 -- mobile
    JOIN cn_contact_email cn_eml             ON cn_eml.contact_id = cn.id
                                              AND cn_eml.discontinue_date IS NULL -- active
                                              AND cn_eml.email_type = 1 -- privateEmail
    JOIN p_cover pc_hcn                      ON p.id = pc_hcn.endorsment_id
                                              AND pc_hcn.parent_cover_id IS NULL
                                              AND pc_hcn.cover_status_id = 20 -- current cover status
    JOIN t_product_line_option tplo_hcn      ON pc_hcn.product_option_id = tplo_hcn.id
    JOIN T_PRODUCT_LINE_OPTION_TYPE tplot_hcn ON tplo_hcn.option_type_id = tplot_hcn.id
    JOIN cn_address c                        ON asp.address_id = c.id -- Join for city_name and zip
WHERE ph.product_id = 1000001 -- Home Policy
  AND ph.status_ID = 20 -- Active policy
  AND ppc.policy_contact_role IN (6, 8) -- Policyholder or co-Policyholder
  AND asp.lodger_type = 1000000 -- Homeowner
  AND p.endors_start_date BETWEEN DATEADD(day, 31, p.policy_start_date) AND CONVERT(date, GETDATE())
  AND p.external_policy_number IS NOT NULL 
  AND cn.first_name IS NOT NULL
  AND cn.name IS NOT NULL
  AND cp.date_of_birth IS NOT NULL
  AND tplot_hcn.external_code IN ('OVS') -- OVS = Outside Personal Valuables Specified | OVU = Outside Personal Valuables Unspecified
ORDER BY NEWID();
