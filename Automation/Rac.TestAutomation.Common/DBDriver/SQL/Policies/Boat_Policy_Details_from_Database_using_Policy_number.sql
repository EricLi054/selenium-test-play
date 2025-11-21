SELECT 
        tu.name_of_user as PolicyUpdateUser
		,p.external_policy_number AS PolicyNumber
        ,ts.DESCRIPTION AS Status
        ---MORE BOAT------------------------------------------------------
		,tc.city_name AS RiskSuburb
		,tz.zip as RiskPostcode
        ,tar.risk_value AS RiskSuburbPoints
		,tic.dsc AS OriginalChannel
        ,td.DESCRIPTION AS Discount
		,ncb.DESCRIPTION AS Ncb
        ,tplot.DESCRIPTION AS Cover
        ,tcm.DESCRIPTION AS PaymentTerms
        ,tpt.DESCRIPTION AS PaymentFreq
        ---YOUR QUOTE-----------------------------------------------------
		,cast(pc.excess_amn as int) as BasicExcess
        ,case when (select count(tplo2.external_code) --, tplo2.description
					FROM    p_pol_header ph2                   
					JOIN    p_policy P2                              ON  ph2.active_policy_id = P2.ID
					JOIN    p_policy_raci pr2                        ON  P2.ID = pr2.ID
					JOIN    p_cover pc2                              ON  P2.ID = pc2.endorsment_id
					JOIN    t_product_line_option tplo2              ON  pc2.product_option_id = tplo2.ID
					where 1=1
					and p2.id = p.id
					and tplo2.external_code = 'MSEO') > 0 then 'True'
			else 'False' end HasWaterSkiingAndFlotationDeviceCover
		,case when (select count(tplo3.external_code) --, tplo2.description
					FROM    p_pol_header ph3                   
					JOIN    p_policy P3                              ON  ph3.active_policy_id = P3.ID
					JOIN    p_policy_raci pr3                        ON  P3.ID = pr3.ID
					JOIN    p_cover pc3                              ON  P3.ID = pc3.endorsment_id
					JOIN    t_product_line_option tplo3              ON  pc3.product_option_id = tplo3.ID
					where 1=1
					and p3.id = p.id
					and tplo3.external_code = 'MREO') > 0 then 'True'
			else 'False' end HasRacingCover
		---START DATE-----------------------------------------------------
		,FORMAT(P.policy_start_date, 'dd/MM/yyyy') AS TermStart
        ,FORMAT(P.policy_end_date, 'dd/MM/yyyy') AS TermEnd
        ,FORMAT(ph.policy_start_date, 'dd/MM/yyyy') AS PolicyCreated
        ---MISC-----------------------------------------------------------
		,asm.has_alarm as OldHasAlarmShouldBeNull
		,asm.has_gps_tracking_device as OldHasGpsShouldBeNull
		,tvu.description as VehicleUsageDscShouldBeNull
		,ph.external_header_number as ExternalHeaderNumber
        ,P.ID AS PolicyVersionId
        ,P.policy_version_nr as PolicyVersionNr
        ,P.endors_nr as EndorsNr
        ,ph.renewal_nr as RenewalNr
FROM            p_pol_header ph                    
        JOIN    p_policy P                          	ON  ph.active_policy_id = P.ID
        JOIN    p_policy_raci pr                    	ON  P.ID = pr.ID
        JOIN    p_cover pc                          	ON  P.ID = pc.endorsment_id
        JOIN    p_policy_lob ppl                    	ON  ph.active_policy_id = ppl.policy_id
        JOIN    p_policy_lob_to_lob_asset ppltla    	ON  ppl.ID = ppltla.policy_lob_id
        JOIN    p_policy_lob_asset ppla             	ON  ppltla.lob_asset_id = ppla.ID
        JOIN    p_policy_lob_asset_raci plar        	ON  ppla.ID = plar.ID
        JOIN    as_asset ass                        	ON  ppla.lob_asset_id = ass.ID
        LEFT JOIN    as_marine_raci asm                  	ON  ass.ID = asm.ID
		LEFT JOIN    as_mortgage mort					ON  mort.asset_id = ass.id
		LEFT JOIN	cn_contact cnmort					ON  cnmort.id = mort.contact_id
		JOIN	t_boat_make_raci tbmr					ON  tbmr.id = asm.boat_make
		JOIN    t_mortgage_types tmt					ON	tmt.id = ass.mortgage_type_id
		JOIN    t_marine_engine_type_raci eng			ON  eng.id = asm.marine_engine_type_id
        JOIN    t_status_code ts                    	ON  ph.status_id = ts.ID
        JOIN    t_policy_discount_grp_raci td       	ON  pr.discount_group_id = td.ID
        JOIN    t_collection_method tcm                 ON  P.collection_method_id = tcm.ID
        JOIN    t_payment_terms tpt                     ON  P.payment_term_id = tpt.ID
        JOIN    t_intermediary_channel tic          	ON  P.original_channel_id = tic.ID
        JOIN    t_product_line_option tplo          	ON  pc.product_option_id = tplo.ID
        JOIN    t_product_line_option_type tplot    	ON  tplot.ID = tplo.option_type_id
        JOIN    t_bm_level_raci ncb                 	ON  asm.ncb_level = ncb.ID
        FULL OUTER JOIN t_vehicle_usage tvu             ON  asm.permitted_usage = tvu.ID
        JOIN    t_boat_type_raci tbtr               	ON  asm.boat_type_id = tbtr.ID
        JOIN    t_construction_type tct             	ON  asm.hull_construction_type_id = tct.ID
        JOIN    t_city tc                           	ON  plar.city_id = tc.ID
		JOIN    t_zip tz                          		ON  tc.id = tz.CITY_ID
        JOIN    t_asset_risk tar                    	ON  plar.city_id = tar.ID
		JOIN    t_user tu								ON  tu.userid = p.update_user
WHERE   1=1
AND     ph.product_id = 1000033 --BGP Boat Product
AND     ph.status_id = 20 --Policy Status
AND     pc.cover_status_id = 20 --Policy Status
AND     pc.parent_cover_id IS NULL --Only Want the parent cover and not the child covers
and		p.external_policy_number = @policynumber
;