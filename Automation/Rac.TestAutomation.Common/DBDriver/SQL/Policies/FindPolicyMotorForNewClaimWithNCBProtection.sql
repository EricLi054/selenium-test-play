/*
PURPOSE: Fetch a list of Full Comprehensive cover motor policies with No
Claim Bonus (NCB) protection and on annual payments. Return either a PH
or coPH reference with the policy.
AUTHOR: Jason King
LAST UPDATE: 2021-12-01
*/
select top 50
       p.external_policy_number   as policy, 
       c.id,
       c.first_name,
       c.name,
       (SELECT top 1 cn_tel.telephone_number FROM cn_contact_telephone cn_tel
        WHERE  cn_tel.contact_id = c.id
          and  cn_tel.discontinue_date is null --active
         and cn_tel.telephone_type = 4 -- mobile
       ) as mobile,
       (SELECT top 1 cn_eml.email FROM cn_contact_email cn_eml
        WHERE  cn_eml.contact_id = c.id
          and  cn_eml.discontinue_date is null --active
          and  cn_eml.email_type = 1 --privateEmail
       ) as email
from   p_policy p 
join   p_pol_header ph                  on  p.id = ph.ACTIVE_POLICY_ID 
JOIN   p_policy_contact ppc             ON  ppc.policy_id = PH.ACTIVE_POLICY_ID
join   p_cover  pc                      on  p.id=pc.ENDORSMENT_ID
join   t_product_line_option tplo       on  pc.product_option_id = tplo.id
join   T_PRODUCT_LINE_OPTION_TYPE tplot on  tplot.id = tplo.option_type_id
JOIN   t_payment_terms tpt              ON  tpt.id = p.payment_term_id
join   cn_contact c                     on c.id = ppc.contact_id
JOIN   cn_person cp                     ON c.id = cp.contact_id and cp.date_of_birth is not null and cp.title is not null
JOIN   P_POLICY_LOB ppl                 ON ppl.POLICY_ID = ph.ACTIVE_POLICY_ID
JOIN   P_POLICY_LOB_TO_LOB_ASSET PPLTLA ON PPL.ID = PPLTLA.POLICY_LOB_ID
JOIN   P_POLICY_LOB_ASSET PPLA          ON PPLTLA.LOB_ASSET_ID = PPLA.ID
JOIN   AS_Asset ass                     ON PPLA.LOB_ASSET_ID = ass.id
JOIN   AS_Vehicle asv                   ON ass.id = asv.Asset_id
JOIN   T_VEHICLE_USAGE tvu              ON tvu.id = asv.use_id
JOIN   AS_VEHICLE_RACI asv_raci         ON asv_raci.ID = asv.ID

where  
     ph.product_id = 1000000 -- MOTOR
 and p.status_id = 20   -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
 and ph.status_id = 20  -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
 and tplot.id = 1000013 --MFCO
 and c.first_name not like '\_%' escape '\'  
 and pc.excess_amn between 1 and 501  -- Arnie tests generate a liability of ~$550, excess should not exceed payout.
 AND ppc.policy_contact_role in (6,8) -- 6=Policyholder 8=co-Policyholder
 AND p.POLICY_START_DATE  < DATEADD(month, -1, convert(date, GETDATE()))
 -- keep claimant's age above 26 to avoid special excess
 AND cp.date_of_birth < DATEADD(year, -26, convert(date, GETDATE()))
 AND tvu.EXTERNAL_CODE = 'Private'
 AND asv_raci.NCB_PROTECTION_FLAG = 1 -- Has NCB protection
 AND asv.BASIC_SUR_AMT between 10000 and 25000
ORDER BY newid()
;