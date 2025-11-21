/*
PURPOSE: Find Owner Occupied B&C home policy.

AUTHOR: Unknown
LAST UPDATED: Jason King 2021-07-15
*/
SELECT TOP 100 p.EXTERNAL_POLICY_NUMBER,
       ppc.CONTACT_ID,
       ca.city_name, 
       c.name, 
       FORMAT(cnp.date_of_birth,'dd/MM/yyyy'),
       ca.zip,
       p.id
       
FROM p_policy p
JOIN p_pol_header ph      ON ph.active_policy_id = p.id
JOIN p_policy_contact ppc ON ppc.policy_id = PH.ACTIVE_POLICY_ID
JOIN cn_contact c         ON c.id = ppc.contact_id
JOIN cn_person cnp        ON cnp.contact_id = ppc.contact_id
JOIN t_product tp         ON  tp.product_id = ph.product_id
JOIN  p_policy_lob ppl                    ON  PH.ACTIVE_POLICY_ID = ppl.policy_id
JOIN  P_POLICY_LOB_TO_LOB_ASSET PPLTLA    ON  PPL.ID = PPLTLA.POLICY_LOB_ID
JOIN  P_POLICY_LOB_ASSET PPLA             ON  PPLTLA.LOB_ASSET_ID = PPLA.ID
JOIN  AS_Asset ass                        ON  PPLA.LOB_ASSET_ID = ass.id
JOIN  AS_Property asp                     ON  ass.id = asp.ASSET_ID
JOIN  cn_address ca                       ON  ca.ID = asp.address_id
JOIN p_cover pc                           ON p.id=pc.ENDORSMENT_ID
join t_product_line_option tplo           on pc.product_option_id = tplo.id 
join T_PRODUCT_LINE_OPTION_TYPE tplot     on tplot.id = tplo.option_type_id
WHERE ph.product_id           = 1000001 -- Home Policy
  AND p.policy_start_date   < DATEADD(month, -1, convert(date, GETDATE()))
  AND ph.status_ID            = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ppc.policy_contact_role in (6,8) -- 6=Policyholder 8=co-Policyholder
  AND asp.lodger_type = 1000000 -- Homeowner
  AND cnp.date_of_birth is not null
  -- Required to have HCN and HB covers
  AND tplot.external_code = 'HCN'
  AND pc.COVER_STATUS_ID = 20 -- Cover_status_id = 20  ensures that the cover is current
  AND (SELECT count(*) from p_cover pc2 join t_product_line_option tplo2 on pc2.product_option_id = tplo2.id 
                       join T_PRODUCT_LINE_OPTION_TYPE tplot2 on tplot2.id = tplo2.option_type_id
              WHERE pc2.PARENT_COVER_ID is null AND p.id=pc2.ENDORSMENT_ID AND tplot2.external_code = 'HB' AND pc2.COVER_STATUS_ID = 20) = 1
ORDER BY newid();