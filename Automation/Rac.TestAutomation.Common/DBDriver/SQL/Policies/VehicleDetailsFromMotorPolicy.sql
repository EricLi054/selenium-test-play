/*
PURPOSE: Retrieve the make/model, registration and fuel type
   details of the insured vehicle of a motor policy 
AUTHOR:  Unknown
LAST UPDATE: Lopa Lenka 2023-11-30
*/

SELECT tmy.MODEL_YEAR as Year,
       ma.description as Make,
       tmr.FAMILY as model_Family,
       tvt.description as Body,
       TVG.DESCRIPTION AS TRANSMISSION,
       (upper(asi.value)) as Registration_Val,
       asv.FUEL_ID as Fuel_Type
FROM p_policy p 
JOIN p_pol_header ph ON ph.active_policy_id = p.id 
JOIN p_policy_lob ppl ON PH.ACTIVE_POLICY_ID = ppl.policy_id 
JOIN P_POLICY_LOB_TO_LOB_ASSET PPLTLA ON PPL.ID = PPLTLA.POLICY_LOB_ID 
JOIN P_POLICY_LOB_ASSET PPLA ON PPLTLA.LOB_ASSET_ID = PPLA.ID 
JOIN AS_Asset ass ON PPLA.LOB_ASSET_ID = ass.id 
JOIN AS_Vehicle asv ON ass.id = asv.Asset_id 
join as_asset_identifier asi ON asi.asset_id = ass.id 
JOIN T_Model tm ON asv.model_id = tm.MODEL_TABLE_CODE 
JOIN t_Model_raci tmr ON tm.model_table_code = tmr.model_table_code 
JOIN T_Model_year tmy ON tm.model_table_code = tmy.model_table_code 
JOIN T_Manufacture ma ON tm.make_id = ma.id 
JOIN t_vehicle_gear tvg ON tm.automatic_model = tvg.id 
JOIN T_VEHICLE_TYPES TVT ON TM.VEHICLE_TYPE_CODE = TVT.ID 
JOIN S_amount sa ON tmy.sur_amt = sa.id 
JOIN P_amount pa ON asv.vehicle_value = pa.id 
WHERE p.external_policy_number = @policynumber
  and asi.identifier_type_id = 10000  -- 10000=License Plate (vehicle registration)