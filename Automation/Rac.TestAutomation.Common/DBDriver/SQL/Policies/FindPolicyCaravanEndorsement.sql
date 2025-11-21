/*
PURPOSE:Retrieve Caravan or Trailer policies eligible for Endorsement.
AUTHOR: Blake - https://rac-wa.atlassian.net/wiki/spaces/ISP/pages/3360063574/Caravan+Endorsement+Online+CEO#Test-Data-Queries
LAST UPDATED: Lopa on 26/09/2024
*/
SELECT TOP 50
        P.external_policy_number AS policy_number
		,ppc.contact_id as contactid
        ,pc.ins_amn AS ins_value
FROM            p_pol_header ph                     
        JOIN    p_policy P                          ON  ph.active_policy_id = P.ID
        JOIN	P_POLICY_CONTACT ppc				ON  p.id = ppc.POLICY_ID and ppc.POLICY_CONTACT_ROLE = 6 
        JOIN    CN_CONTACT_RACI cnr                 ON  cnr.id = ppc.contact_id
        JOIN    p_policy_lob ppl                    ON  ph.active_policy_id = ppl.policy_id
        JOIN    p_policy_lob_to_lob_asset ppltla    ON  ppl.ID = ppltla.policy_lob_id
        JOIN    p_policy_lob_asset ppla             ON  ppltla.lob_asset_id = ppla.ID 
        JOIN    as_asset ass                        ON  ppla.lob_asset_id = ass.ID
        JOIN    as_vehicle asv                      ON  ass.ID = asv.asset_id
        JOIN    as_vehicle_raci asvr                ON  asv.ID = asvr.ID
        JOIN    p_amount pa                         ON  asv.total_accessories_value = pa.ID
		JOIN    p_cover pc                          ON  p.id = pc.ENDORSMENT_ID AND  pc.parent_cover_id IS NULL AND pc.cover_status_id = 20
WHERE   1=1
AND ph.product_id = 1000008 --MGV Caravan Product
AND ph.status_id = 20 --Policy Status
AND asvr.program_type_id = @vehicleType --Caravan =1000002 Trailer=1000003
AND pc.ins_amn between @valueMin and @valueMax
AND pc.PRODUCT_LINE_TYPE = 1000087 -- This is the "vehicle" (caravan) insured cover
--This section is for MidTerm policies 
AND CONVERT(DATETIME, CONVERT(DATE, P.endors_start_date)) < CONVERT(DATETIME, GETDATE()) --This is to find policies with no Future Dated Endorsement
AND CONVERT(DATETIME, CONVERT(DATE, P.policy_end_date)) > CONVERT(DATETIME, CONVERT(DATE, DATEADD(DAY, 90, GETDATE())))
--The line below allows you to search by Collection Method
AND p.collection_method_id = @collectionmethod -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
--The line below allows you to search by Payment Frequency
AND P.payment_term_id =  @paymentterm -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
AND (cnr.WESTPAC_CUSTOMER_ID   = ppc.contact_id or cnr.WESTPAC_CUSTOMER_ID is null) -- Won't be impacted by the issue described in https://rac-wa.atlassian.net/wiki/spaces/SS/pages/2919465010/Westpac+Tokenisation+of+Non-Prod+Environments
ORDER BY newid();