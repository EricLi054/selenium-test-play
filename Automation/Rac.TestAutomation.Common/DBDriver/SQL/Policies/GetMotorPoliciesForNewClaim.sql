/*
PURPOSE: Fetch a combine list of Full NonEV and EV motor policies with no
current claims and private usage type. Return either a PH or coPH 
reference with the policy.
AUTHOR: unknown
LAST UPDATE: Jason King 2024-08-01
*/
SELECT * FROM 
(
SELECT DISTINCT TOP 500
       p.external_policy_number AS policyNumber,
	   tplot.id AS motorCover,
       asv.FUEL_ID as fuelType,
	   asid.Value as Rego,
	   (SELECT MIN(POLICY_START_DATE) FROM P_POLICY pp
		WHERE pp.EXTERNAL_POLICY_NUMBER = p.EXTERNAL_POLICY_NUMBER) AS PolicyStartDate
FROM   p_policy p 
JOIN   p_pol_header ph                  ON  p.id = ph.ACTIVE_POLICY_ID 
JOIN   p_policy_contact ppc             ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
JOIN   p_cover  pc                      ON  p.id=pc.ENDORSMENT_ID
JOIN   p_policy_lob ppl                    ON  ph.active_policy_id = ppl.policy_id
JOIN   p_policy_lob_to_lob_asset ppltla    ON  ppl.ID = ppltla.policy_lob_id
JOIN   p_policy_lob_asset ppla             ON  ppltla.lob_asset_id = ppla.ID
JOIN   as_asset ass                        ON  ppla.lob_asset_id = ass.ID
JOIN   as_asset_identifier asid            ON  ass.ID = asid.asset_id  AND asid.IDENTIFIER_TYPE_ID = 10000 --Licence Plate
JOIN   as_vehicle asv                      ON  ass.ID = asv.asset_id
JOIN   t_product_line_option tplo       ON  pc.product_option_id = tplo.id
JOIN   T_PRODUCT_LINE_OPTION_TYPE tplot ON  tplot.id = tplo.option_type_id
JOIN   cn_contact c                     ON c.id = ppc.contact_id
JOIN   cn_person cp                     ON c.id = cp.contact_id AND cp.date_of_birth IS NOT NULL AND cp.title IS NOT NULL
WHERE  
     ph.product_id = 1000000 -- MOTOR
 AND p.status_id = 20   -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
 AND ph.status_id = 20  -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
 AND tplot.id = '1000013'
 AND c.first_name NOT LIKE '\_%' ESCAPE '\'
 AND ppc.policy_contact_role IN (6,8) -- 6=Policyholder 8=co-Policyholder
 AND p.POLICY_START_DATE  < DATEADD(month, -1, CONVERT(date, GETDATE()))
 AND p.POLICY_END_DATE > CONVERT(date, GETDATE())
 AND asv.FUEL_ID != '4000001' -- 4000001 of Fuel type Electric
 AND asid.Value NOT IN ('TBA', 'TBC')
Union
SELECT DISTINCT TOP 100
        p.external_policy_number AS policyNumber,
	   tplot.id AS motorCover,
       asv.FUEL_ID as fuelType,
	   asid.Value as Rego,
	   (SELECT MIN(POLICY_START_DATE) FROM P_POLICY pp
		WHERE pp.EXTERNAL_POLICY_NUMBER = p.EXTERNAL_POLICY_NUMBER) AS PolicyStartDate
FROM   p_policy p 
JOIN   p_pol_header ph                  ON  p.id = ph.ACTIVE_POLICY_ID 
JOIN   p_policy_contact ppc             ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
JOIN   p_cover  pc                      ON  p.id=pc.ENDORSMENT_ID
JOIN   p_policy_lob ppl                    ON  ph.active_policy_id = ppl.policy_id
JOIN   p_policy_lob_to_lob_asset ppltla    ON  ppl.ID = ppltla.policy_lob_id
JOIN   p_policy_lob_asset ppla             ON  ppltla.lob_asset_id = ppla.ID
JOIN   as_asset ass                        ON  ppla.lob_asset_id = ass.ID
JOIN   as_asset_identifier asid            ON  ass.ID = asid.asset_id  AND asid.IDENTIFIER_TYPE_ID = 10000 --Licence Plate
JOIN   as_vehicle asv                      ON  ass.ID = asv.asset_id
JOIN   t_product_line_option tplo       ON  pc.product_option_id = tplo.id
JOIN   T_PRODUCT_LINE_OPTION_TYPE tplot ON  tplot.id = tplo.option_type_id
JOIN   cn_contact c                     ON c.id = ppc.contact_id
JOIN   cn_person cp                     ON c.id = cp.contact_id AND cp.date_of_birth IS NOT NULL AND cp.title IS NOT NULL
WHERE  
     ph.product_id = 1000000 -- MOTOR
 AND p.status_id = 20   -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
 AND ph.status_id = 20  -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
 AND tplot.id = '1000017'
 AND c.first_name NOT LIKE '\_%' ESCAPE '\'
 AND ppc.policy_contact_role IN (6,8) -- 6=Policyholder 8=co-Policyholder
 AND p.POLICY_START_DATE  < DATEADD(month, -1, CONVERT(date, GETDATE()))
 AND p.POLICY_END_DATE > CONVERT(date, GETDATE())
 AND asv.FUEL_ID != '4000001' -- 4000001 of Fuel type Electric
 AND asid.Value NOT IN ('TBA', 'TBC')
Union
SELECT DISTINCT TOP 100
       p.external_policy_number AS policyNumber,
	   tplot.id AS motorCover,
       asv.FUEL_ID as fuelType,
	   asid.Value as Rego,
	   (SELECT MIN(POLICY_START_DATE) FROM P_POLICY pp
		WHERE pp.EXTERNAL_POLICY_NUMBER = p.EXTERNAL_POLICY_NUMBER) AS PolicyStartDate
FROM   p_policy p 
JOIN   p_pol_header ph                  ON  p.id = ph.ACTIVE_POLICY_ID 
JOIN   p_policy_contact ppc             ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
JOIN   p_cover  pc                      ON  p.id=pc.ENDORSMENT_ID
JOIN   p_policy_lob ppl                    ON  ph.active_policy_id = ppl.policy_id
JOIN   p_policy_lob_to_lob_asset ppltla    ON  ppl.ID = ppltla.policy_lob_id
JOIN   p_policy_lob_asset ppla             ON  ppltla.lob_asset_id = ppla.ID
JOIN   as_asset ass                        ON  ppla.lob_asset_id = ass.ID
JOIN   as_asset_identifier asid            ON  ass.ID = asid.asset_id  AND asid.IDENTIFIER_TYPE_ID = 10000 --Licence Plate
JOIN   as_vehicle asv                      ON  ass.ID = asv.asset_id
JOIN   t_product_line_option tplo       ON  pc.product_option_id = tplo.id
JOIN   T_PRODUCT_LINE_OPTION_TYPE tplot ON  tplot.id = tplo.option_type_id
JOIN   cn_contact c                     ON c.id = ppc.contact_id
JOIN   cn_person cp                     ON c.id = cp.contact_id AND cp.date_of_birth IS NOT NULL AND cp.title IS NOT NULL
WHERE  
     ph.product_id = 1000000 -- MOTOR
 AND p.status_id = 20   -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
 AND ph.status_id = 20  -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
 AND tplot.id = '1000018'
 AND c.first_name NOT LIKE '\_%' ESCAPE '\'
 AND ppc.policy_contact_role IN (6,8) -- 6=Policyholder 8=co-Policyholder
 AND p.POLICY_START_DATE  < DATEADD(month, -1, CONVERT(date, GETDATE()))
 AND p.POLICY_END_DATE > CONVERT(date, GETDATE())
 AND asv.FUEL_ID != '4000001' -- 4000001 of Fuel type Electric
 AND asid.Value NOT IN ('TBA', 'TBC')
Union
SELECT DISTINCT TOP 50
       p.external_policy_number AS policyNumber,
	   tplot.id AS motorCover,
       asv.FUEL_ID as fuelType,
	   asid.Value as Rego,
	  (SELECT MIN(POLICY_START_DATE) FROM P_POLICY pp
		WHERE pp.EXTERNAL_POLICY_NUMBER = p.EXTERNAL_POLICY_NUMBER) AS PolicyStartDate
	FROM   p_policy p 
	JOIN   p_pol_header ph                  ON  p.id = ph.ACTIVE_POLICY_ID 
	JOIN   p_policy_contact ppc             ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
	JOIN   p_cover  pc                      ON  p.id=pc.ENDORSMENT_ID
	JOIN   p_policy_lob ppl                    ON  ph.active_policy_id = ppl.policy_id
	JOIN   p_policy_lob_to_lob_asset ppltla    ON  ppl.ID = ppltla.policy_lob_id
	JOIN   p_policy_lob_asset ppla             ON  ppltla.lob_asset_id = ppla.ID
	JOIN   as_asset ass                        ON  ppla.lob_asset_id = ass.ID
	JOIN   as_asset_identifier asid            ON  ass.ID = asid.asset_id  AND asid.IDENTIFIER_TYPE_ID = 10000 --Licence Plate
	JOIN   as_vehicle asv                      ON  ass.ID = asv.asset_id
	JOIN   t_product_line_option tplo       ON  pc.product_option_id = tplo.id
	JOIN   T_PRODUCT_LINE_OPTION_TYPE tplot ON  tplot.id = tplo.option_type_id
	JOIN   cn_contact c                     ON c.id = ppc.contact_id
	JOIN   cn_person cp                     ON c.id = cp.contact_id and cp.date_of_birth IS NOT NULL AND cp.title IS NOT NULL
	WHERE  ph.product_id = 1000000 -- MOTOR
	AND p.status_id = 20   -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
	 AND ph.status_id = 20  -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
	 AND c.first_name not like '\_%' escape '\' 
	 AND ppc.policy_contact_role IN (6,8) -- 6=Policyholder 8=co-Policyholder
	 AND p.POLICY_START_DATE  < DATEADD(month, -1, CONVERT(date, GETDATE()))
	 AND p.POLICY_END_DATE > CONVERT(date, GETDATE())
	 AND asv.FUEL_ID = '4000001' -- 4000001 of Fuel type Electric
	 AND tplo.option_type_id in (1000013, 1000017, 1000018)
	 AND asid.Value NOT IN ('TBA', 'TBC')
Union
SELECT DISTINCT TOP 10
       p.external_policy_number AS policyNumber,
	   tplot.id AS motorCover,
       asv.FUEL_ID as fuelType,
	   asid.Value as Rego,
	   (SELECT MIN(POLICY_START_DATE) FROM P_POLICY pp
		WHERE pp.EXTERNAL_POLICY_NUMBER = p.EXTERNAL_POLICY_NUMBER) AS PolicyStartDate
	FROM   p_policy p 
	JOIN   p_pol_header ph                  ON  p.id = ph.ACTIVE_POLICY_ID 
	JOIN   p_policy_contact ppc             ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
	JOIN   p_cover  pc                      ON  p.id=pc.ENDORSMENT_ID
	JOIN   p_policy_lob ppl                    ON  ph.active_policy_id = ppl.policy_id
	JOIN   p_policy_lob_to_lob_asset ppltla    ON  ppl.ID = ppltla.policy_lob_id
	JOIN   p_policy_lob_asset ppla             ON  ppltla.lob_asset_id = ppla.ID
	JOIN   as_asset ass                        ON  ppla.lob_asset_id = ass.ID
	JOIN   as_asset_identifier asid            ON  ass.ID = asid.asset_id  AND asid.IDENTIFIER_TYPE_ID = 10000 --Licence Plate
	JOIN   as_vehicle asv                      ON  ass.ID = asv.asset_id
	JOIN   t_product_line_option tplo       ON  pc.product_option_id = tplo.id
	JOIN   T_PRODUCT_LINE_OPTION_TYPE tplot ON  tplot.id = tplo.option_type_id
	JOIN   cn_contact c                     ON c.id = ppc.contact_id
	JOIN   cn_person cp                     ON c.id = cp.contact_id and cp.date_of_birth IS NOT NULL AND cp.title IS NOT NULL
	WHERE ph.product_id = 1000000 -- MOTOR
	 AND p.status_id = 20   -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
	 AND ph.status_id = 20  -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
	 AND c.first_name not like '\_%' escape '\' 
	 AND ppc.policy_contact_role IN (6,8) -- 6=Policyholder 8=co-Policyholder
	 AND p.POLICY_START_DATE  < DATEADD(month, -1, CONVERT(date, GETDATE()))
	 AND p.POLICY_END_DATE > CONVERT(date, GETDATE())
     AND tplo.option_type_id in (1000013, 1000017, 1000018)
	 AND asid.Value  IN ('TBA', 'TBC')
) p
ORDER BY NEWID();