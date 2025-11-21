/*
PURPOSE: Find Landlord and Homeowner building home policy.
*/

SELECT EXTERNAL_POLICY_NUMBER
	,CONTACT_ID
	,external_code
	,city_name
	,[name]
	,date_of_birth
	,zip
	,id
FROM (
	SELECT p.EXTERNAL_POLICY_NUMBER
		,ppc.CONTACT_ID
		,tplot.external_code
		,ca.city_name
		,c.name
		,FORMAT(cnp.date_of_birth, 'dd/MM/yyyy') AS date_of_birth
		,ca.zip
		,p.id
		,row_number() OVER (
			PARTITION BY tplot.external_code ORDER BY NEWID()
			) AS rownumber
	FROM p_policy p
	JOIN p_pol_header ph ON ph.active_policy_id = p.id
	JOIN p_policy_contact ppc ON ppc.policy_id = PH.ACTIVE_POLICY_ID
	JOIN cn_contact c ON c.id = ppc.contact_id
	JOIN cn_person cnp ON cnp.contact_id = ppc.contact_id
	JOIN t_product tp ON tp.product_id = ph.product_id
	JOIN p_policy_lob ppl ON PH.ACTIVE_POLICY_ID = ppl.policy_id
	JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA ON PPL.ID = PPLTLA.POLICY_LOB_ID
	JOIN P_POLICY_LOB_ASSET PPLA ON PPLTLA.LOB_ASSET_ID = PPLA.ID
	JOIN AS_Asset ass ON PPLA.LOB_ASSET_ID = ass.id
	JOIN AS_Property asp ON ass.id = asp.ASSET_ID
	JOIN cn_address ca ON ca.ID = asp.address_id
	JOIN p_cover pc ON p.id = pc.ENDORSMENT_ID
	JOIN t_product_line_option tplo ON pc.product_option_id = tplo.id
	JOIN T_PRODUCT_LINE_OPTION_TYPE tplot ON tplot.id = tplo.option_type_id
	WHERE ph.product_id = 1000001 -- Home Policy
		AND p.policy_start_date < DATEADD(month, - 1, convert(DATE, GETDATE()))
		AND p.ENDORS_START_DATE < DATEADD(day, -21, convert(DATE, GETDATE()))
		AND ph.status_ID = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
		AND ppc.policy_contact_role IN (6,8,1000000) -- 6=Policyholder 8=co-Policyholder 1000000 = Auth party
		--AND asp.lodger_type = 1000000 -- Homeowner
		AND cnp.date_of_birth IS NOT NULL
		-- Required to have LB and HB covers
		AND tplot.external_code IN ('LB','HB')
		AND pc.COVER_STATUS_ID = 20 -- Cover_status_id = 20  ensures that the cover is current  
	GROUP BY p.EXTERNAL_POLICY_NUMBER
		,ppc.CONTACT_ID
		,tplot.external_code
		,ca.city_name
		,c.name
		,FORMAT(cnp.date_of_birth, 'dd/MM/yyyy')
		,ca.zip
		,p.id
	) nkm
WHERE rownumber < 100;