/*
PURPOSE: Find a current home policy that is not in renewal and in low risk suburb.
Intent is to be used in a endorsement test where the premium increases when
the new suburb is changed to a high risk suburb.
LAST UPDATED: Jason King 2021-07-13
*/
SELECT TOP 200 p.external_policy_number as Policy_Number
              ,ppc.contact_id
              ,ph.policy_end_date
              ,ph.renewal_date
FROM p_policy p
JOIN p_pol_header ph      ON ph.active_policy_id = p.id
JOIN p_policy_contact ppc ON ppc.policy_id = PH.ACTIVE_POLICY_ID
JOIN t_product tp         ON  tp.product_id = ph.product_id
JOIN  p_policy_lob ppl                    ON  PH.ACTIVE_POLICY_ID = ppl.policy_id
JOIN  P_POLICY_LOB_TO_LOB_ASSET PPLTLA    ON  PPL.ID = PPLTLA.POLICY_LOB_ID
JOIN  P_POLICY_LOB_ASSET PPLA             ON  PPLTLA.LOB_ASSET_ID = PPLA.ID
JOIN  AS_Asset ass                        ON  PPLA.LOB_ASSET_ID = ass.id
JOIN  AS_Property asp                     ON  ass.id = asp.ASSET_ID
JOIN  cn_address cna                      on  cna.id = asp.address_id
JOIN  P_POLICY_LOB_ASSET_RACI PLAR        ON  ppla.ID = plar.id
JOIN  t_asset_risk tar                    ON  tar.id = plar.city_id
JOIN  t_city tc                           ON  tc.ID = plar.CITY_ID
JOIN  t_zip tz                            ON  tz.CITY_ID = tc.id
WHERE ph.product_id           = 1000001 -- Home Policy
  AND ph.policy_end_date      between DATEADD(month, 6, convert(date, GETDATE())) and DATEADD(month, 11, convert(date, GETDATE())) -- Sufficient time to show premium change over $10
  AND ph.status_ID            = 20  -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ppc.policy_contact_role = 6   -- 6=Policyholder 8=co-Policyholder
  AND asp.lodger_type = 1000000   -- Homeowner
  AND p.collection_method_ID  = 1 -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
  AND p.payment_term_id       = 6 -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
  AND tar.DISCONTINUE_DATE IS NULL --Discontinued Suburb Points
  AND tc.discontinue_date IS NULL --Discontinued Cities
  AND tz.DISCONTINUE_DATE IS NULL --Discontinued Postcodes
  AND tar.risk_value < 30 --Suburbs that are priced low
  AND cna.house_nr is not null
  AND cna.street_name is not null
  AND tz.zip < 6699
       -- Cover_status_id = 20  ensures that the cover is current
  AND (SELECT count(*) from p_cover pc join t_product_line_option tplo on pc.product_option_id = tplo.id join T_PRODUCT_LINE_OPTION_TYPE tplot on tplot.id = tplo.option_type_id
       WHERE pc.PARENT_COVER_ID is null AND p.id=pc.ENDORSMENT_ID AND tplot.external_code = 'HCN' AND pc.COVER_STATUS_ID = 20) = 1
  AND (SELECT count(*) from p_cover pc join t_product_line_option tplo on pc.product_option_id = tplo.id join T_PRODUCT_LINE_OPTION_TYPE tplot on tplot.id = tplo.option_type_id
       WHERE pc.PARENT_COVER_ID is null AND p.id=pc.ENDORSMENT_ID AND tplot.external_code = 'HB' AND pc.COVER_STATUS_ID = 20) = 1
ORDER BY newid();