/*
PURPOSE: Find a current home policy that is not in renewal and in high risk suburb.
Intent is to be used in a endorsement test where the premium decreases when
the new suburb is changed to a low risk suburb.
LAST UPDATED: Jason King 2021-07-13
*/
SELECT TOP 200 p.external_policy_number as Policy_Number
			  ,ppc.contact_id
FROM p_policy p
JOIN p_pol_header ph					ON ph.active_policy_id = p.id
JOIN p_policy_lob ppl					ON PH.ACTIVE_POLICY_ID = ppl.policy_id
JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA 	ON PPL.ID = PPLTLA.POLICY_LOB_ID
JOIN P_POLICY_LOB_ASSET PPLA 			ON PPLTLA.LOB_ASSET_ID = PPLA.ID
JOIN P_POLICY_LOB_ASSET_RACI PLAR 		ON PPLTLA.LOB_ASSET_ID = plar.id
JOIN p_policy_contact ppc 				ON ppc.policy_id = PH.ACTIVE_POLICY_ID
JOIN t_asset_risk tar 					ON tar.id = plar.city_id
JOIN AS_Asset ass 						ON PPLA.LOB_ASSET_ID = ass.id
JOIN AS_Property asp 					ON ass.id = asp.ASSET_ID
JOIN cn_address pa 						ON pa.id = asp.ADDRESS_ID
JOIN cn_contact c 						ON c.id = ppc.contact_id
JOIN cn_person cnp 						ON cnp.contact_id = ppc.contact_id
JOIN t_city tc                          ON tc.ID = plar.CITY_ID
JOIN t_zip tz                           ON tz.CITY_ID = tc.id
WHERE ph.product_id = 1000001 --HGP
	AND ph.status_ID = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
	-- Has Landord's Building but not Contents. cover_status_id = 20  ensures that the cover is current
	AND (SELECT count(*) from p_cover pc join t_product_line_option tplo on pc.product_option_id = tplo.id join T_PRODUCT_LINE_OPTION_TYPE tplot on tplot.id = tplo.option_type_id
         WHERE pc.PARENT_COVER_ID is null AND p.id=pc.ENDORSMENT_ID AND tplot.external_code = 'LCN') = 0
	AND (SELECT count(*) from p_cover pc2 join t_product_line_option tplo2 on pc2.product_option_id = tplo2.id join T_PRODUCT_LINE_OPTION_TYPE tplot2 on tplot2.id = tplo2.option_type_id
         WHERE pc2.PARENT_COVER_ID is null AND p.id=pc2.ENDORSMENT_ID AND tplot2.external_code = 'LB' AND pc2.COVER_STATUS_ID = 20) = 1
	AND pa.street_name is not null
	AND p.collection_method_ID = 2  -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
	AND p.payment_term_id = 4       -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
	AND ppc.policy_contact_role = 6 -- 6=Policyholder 8=co-Policyholder
	AND p.PROPOSAL_DATE < DATEADD(DAY, -40, GETDATE())
	AND p.policy_end_date between DATEADD(DAY, 40, convert(date, GETDATE())) and DATEADD(DAY, 320, convert(date, GETDATE()))
	AND tar.risk_value > 50 --Suburbs that are priced sort of high
	AND tz.zip < 6699
	AND cnp.date_of_birth is not null
	AND c.name is not null
	AND c.FIRST_NAME is not null
	AND cnp.gender is not null
	AND c.name not like '%Estate %'
ORDER BY newid();