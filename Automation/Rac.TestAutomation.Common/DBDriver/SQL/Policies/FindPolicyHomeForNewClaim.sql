/*
PURPOSE: Find Home Policy to report a claim against.

AUTHOR: Unknown
LAST UPDATED: Troy Hall 2024-07-03
*/
SELECT TOP 100 ppc.CONTACT_ID, p.EXTERNAL_POLICY_NUMBER
FROM p_policy p
JOIN p_pol_header ph      ON ph.active_policy_id = p.id
JOIN p_policy_contact ppc ON ppc.policy_id = PH.ACTIVE_POLICY_ID
JOIN t_product tp         ON  tp.product_id = ph.product_id
JOIN  p_policy_lob ppl                    ON  PH.ACTIVE_POLICY_ID = ppl.policy_id
JOIN  P_POLICY_LOB_TO_LOB_ASSET PPLTLA    ON  PPL.ID = PPLTLA.POLICY_LOB_ID
JOIN  P_POLICY_LOB_ASSET PPLA             ON  PPLTLA.LOB_ASSET_ID = PPLA.ID
JOIN  AS_Asset ass                        ON  PPLA.LOB_ASSET_ID = ass.id
JOIN  AS_Property asp                     ON  ass.id = asp.ASSET_ID
WHERE ph.product_id           = 1000001 -- Home Policy
  AND ph.policy_end_date    > DATEADD(month, 1, convert(date, GETDATE())) -- Don't want to be in renewal
  AND ph.status_ID            = 20  -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ppc.policy_contact_role in (6,8) -- 6=Policyholder 8=co-Policyholder
  AND asp.lodger_type = 1000000 -- Homeowner
       -- cover_status_id = 20  ensures that the cover is current
       -- Must have Building, Contents, Specified Valuables covers
  AND (SELECT count(*) from p_cover pc join t_product_line_option tplo on pc.product_option_id = tplo.id join T_PRODUCT_LINE_OPTION_TYPE tplot on tplot.id = tplo.option_type_id
       WHERE pc.PARENT_COVER_ID is null AND p.id=pc.ENDORSMENT_ID AND tplot.external_code in ('HCN','HB','OVS')  AND pc.COVER_STATUS_ID = 20) = 3
ORDER BY newid();