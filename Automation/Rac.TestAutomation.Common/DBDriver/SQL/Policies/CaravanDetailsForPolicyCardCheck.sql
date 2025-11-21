/*
PURPOSE: Retrieve the make/model and registration
   details of the insured asset of a caravan or trailer policy 
AUTHOR:  James Barton
LAST UPDATE: James Barton 2022-12-23
*/

SELECT   TOP 1 tmy.model_year    AS Year
        ,ma.DESCRIPTION          AS Make
        ,tm.dsc                  AS Model
        ,(upper(asi.value))      AS Registration
        ,tlt.DESCRIPTION         AS Type
FROM    p_pol_header ph                     
        JOIN    p_policy P                          ON  ph.active_policy_id = P.ID
        JOIN    p_cover pc                          ON  ph.active_policy_id = pc.endorsment_id
        JOIN    t_product_line_option tplo          ON  tplo.id = pc.product_option_id AND pc.PARENT_COVER_ID is NULL
        JOIN    p_policy_raci pr                    ON  p.id = pr.ID
        JOIN    p_policy_lob ppl                    ON  ph.active_policy_id = ppl.policy_id
        JOIN    p_policy_lob_to_lob_asset ppltla    ON  ppl.ID = ppltla.policy_lob_id
        JOIN    p_policy_lob_asset ppla             ON  ppltla.lob_asset_id = ppla.ID 
        JOIN    as_asset ass                        ON  ppla.lob_asset_id = ass.ID
        JOIN    as_asset_identifier asi             ON  asi.asset_id = ass.id  AND asi.IDENTIFIER_TYPE_ID = 10000 -- Licence Plate
        JOIN    as_vehicle asv                      ON  ass.ID = asv.asset_id
        JOIN    as_vehicle_raci asvr                ON  asv.ID = asvr.ID
        JOIN    t_lodger_type tlt                   ON  asvr.program_type_id = tlt.ID
        JOIN    t_model tm                          ON  asv.model_id = tm.model_table_code
        JOIN    t_model_year tmy                    ON  tm.model_table_code = tmy.model_table_code
        JOIN    t_manufacture ma                    ON  tm.make_id = ma.ID 
WHERE p.external_policy_number = @policynumber
        AND     ph.product_id = 1000008 --MGV Caravan Product
        AND     ph.status_id = 20 --Policy Status
        AND     pc.cover_status_id = 20 --Policy Status
