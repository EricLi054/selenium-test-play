/*
PURPOSE: Fetch endorsement details for Home Policy
         to match against changed details.

AUTHOR: Unknown
LAST UPDATED: Jason King 2024-07-08
*/
SELECT   ca.house_nr
        ,ca.street_name as street
        ,ca.city_name as Suburb
        ,tbt.description as building_type
        ,tpat.description as alarm_type
        ,twstr.external_code as Window_Security
        ,tdstr.external_code as Door_Security
        ,trm.DESCRIPTION as Roof_Material
        ,tct.DESCRIPTION as Construction_Type
        ,asp.BUILDING_YEAR as Year
FROM               p_policy p
                   JOIN  p_pol_header ph                     ON  ph.active_policy_id = p.id
                   JOIN  p_policy_lob ppl                    ON  PH.ACTIVE_POLICY_ID = ppl.policy_id
                   JOIN  P_POLICY_LOB_TO_LOB_ASSET PPLTLA    ON  PPL.ID = PPLTLA.POLICY_LOB_ID
                   JOIN  P_POLICY_LOB_ASSET PPLA             ON  PPLTLA.LOB_ASSET_ID = PPLA.ID 
                   JOIN  AS_Asset ass                        ON  PPLA.LOB_ASSET_ID = ass.id
                   JOIN  AS_Property asp                     ON  ass.id = asp.ASSET_ID
                   LEFT JOIN  T_ROOF_MATERIAL trm            ON  asp.ROOF_MATERIAL = trm.ID  -- B2C-4951 no longer required
                   JOIN  T_CONSTRUCTION_TYPE tct             ON  asp.CONSTRUCTION_TYPE = tct.ID
                   JOIN  AS_property_raci aspr               ON  aspr.id = asp.ASSET_ID
                   JOIN  cn_address ca                       ON  ca.id = asp.ADDRESS_ID
                   JOIN  T_BUILDING_TYPE tbt                 ON  tbt.id = asp.BUILDING_TYPE 
                   JOIN  T_PROPERTY_ALARM_TYPE tpat          ON  tpat.id = asp.ALARM_TYPE_ID
                   JOIN  T_WINDOW_SECURITY_TYPE_RACI twstr   ON  twstr.id = aspr.WINDOW_SECURITY_TYPE_ID
                   JOIN  T_DOOR_SECURITY_TYPE_RACI tdstr     ON  tdstr.id = aspr.DOOR_SECURITY_TYPE_ID
WHERE p.external_policy_number = @policynumber
;