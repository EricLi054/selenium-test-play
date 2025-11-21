/*
PURPOSE: Find Contacts with Building & Contents Policies and select one to 
report a claim against in automated tests.
AUTHOR:  Jason King
LAST UPDATE: Troy Hall 2021-11-24

NOTES:
- The aliases match the names of the buffers used in the TOSCA tests.
- I'm attempting to update the existing query to remove dependency on the 
  METADATA-EXPORT process/tables.
- When inputting this type of data elsewhere we have a ResusableTestStepBlock in 
  Tosca called "Assemble Dynamic Address_Reference" which calls the buffers
  "{B[HOUSE_NR]} {B[STREET]}, {B[CITY]}"
- The most recent policy Endorsement date is used as a reference point for our
  event date, however it is assumed that the DD field will be set to "1" to take
  advantage of the NPE loophole which avoids checks for contemporary claims.
- Unlike Motor claims where we just need to confirm it's a Full Cover policy,
  we check that Storm cover (product line options) exist for both Building & 
  Contents.
- mobilePhoneNumber and privateEmail were added during delivery of Benang One Way Sync
- This query was changed to base on actual query found in Tosca tests as that had diverted from query in this repo.
*/

SELECT TOP 20
    cn.id                                     as ContactId,
    p.external_policy_number                  as PolicyNumber,
    cn.first_name                             as FirstName,
    cn.name                                   as LastName,
    format(cp.date_of_birth,'dd/MM/yyyy')    as DOB,
    format(p.endors_start_date,'dd/MM/yyyy') as PolicyEndorsedDate,
    format(p.endors_start_date,'dd/MM/yyyy') as PolicyEndorsedDateString,
    format(cp.date_of_birth,'dd')           as dob_dd,
    format(cp.date_of_birth,'MM')           as dob_mm,
    format(cp.date_of_birth,'yyyy')         as dob_yyyy,
    (
        select TOP 1
            cna.house_nr + ' ' + cna.street_name + ', ' + cna.city_name 
        from 
            p_cover pc 
            join p_policy_lob_asset ppla on ppla.id = pc.asset_id 
            join as_property asprop on asprop.asset_id = ppla.lob_asset_id 
            join cn_address cna on cna.id = asprop.address_id
        where ph.active_policy_id = pc.endorsment_id
    ) as "AssembledAddress",
    cn_tel.telephone_number                                         as "mobilePhoneNumber",
    cn_eml.email                                                    as "privateEmail"
FROM p_policy p
    join p_pol_header ph      ON ph.active_policy_id = p.id
    join p_policy_contact ppc ON ppc.policy_id = PH.ACTIVE_POLICY_ID
    join t_product tp         ON  tp.product_id = ph.product_id
    join p_policy_lob ppl                    on  PH.ACTIVE_POLICY_ID = ppl.policy_id
    join P_POLICY_LOB_TO_LOB_ASSET PPLTLA    on  PPL.ID = PPLTLA.POLICY_LOB_ID
    join P_POLICY_LOB_ASSET PPLA             on  PPLTLA.LOB_ASSET_ID = PPLA.ID
    join AS_Asset ass                        on  PPLA.LOB_ASSET_ID = ass.id
    join AS_Property asp                     on  ass.id = asp.ASSET_ID
    join cn_contact cn                       on  ppc.contact_id = cn.id 
    join cn_person cp                        on cp.contact_id = cn.id
    join cn_contact_telephone cn_tel         on cn_tel.contact_id = cn.id 
                                            and cn_tel.discontinue_date is null --active
                                            and cn_tel.telephone_type = 4 -- mobile
    join cn_contact_email cn_eml             on cn_eml.contact_id = cn.id
                                            and cn_eml.discontinue_date is null --active
                                            and cn_eml.email_type = 1 --privateEmail
    join p_cover pc_hcn                      on p.id = pc_hcn.ENDORSMENT_ID
                                            and pc_hcn.PARENT_COVER_ID is null
                                            and pc_hcn.COVER_STATUS_ID = 20 -- Cover_status_id = 20  ensures that the cover is current
    join t_product_line_option tplo_hcn      on pc_hcn.product_option_id = tplo_hcn.id
    join T_PRODUCT_LINE_OPTION_TYPE tplot_hcn on tplot_hcn.id = tplo_hcn.option_type_id

WHERE ph.product_id           = 1000001 -- Home Policy
  AND ph.status_ID            = 20      -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ppc.policy_contact_role in (6,8)  -- 6=Policyholder 8=co-Policyholder
  AND asp.lodger_type = 1000000 -- Homeowner
  and p.policy_version_nr > 5
      -- Last endorsement must be at least a month after policy start, and before today (not a future dated endorsement like renewal)
      -- (31 day period to allow use of the NPE loophole which ignores existing claim checks if Event Date = 01/##/####)
  and p.endors_start_date between DATEADD(day, 31, p.policy_start_date) and convert(date, GETDATE()) -- We derive claim date from this, and don't want a future date.
  and p.external_policy_number is not null 
  and cn.first_name is not null --Reduced chance of Deceased Estate being returned
  and cn.name is not null --Reduced chance of Deceased Estate being returned
  and cp.date_of_birth is not null --Humans only please.
  and tplot_hcn.external_code = 'HCN'
       -- Cover_status_id = 20  ensures that the cover is current
  AND (SELECT count(0) from p_cover pc_hb join t_product_line_option tplo_hb on pc_hb.product_option_id = tplo_hb.id join T_PRODUCT_LINE_OPTION_TYPE tplot_hb on tplot_hb.id = tplo_hb.option_type_id
       WHERE pc_hb.PARENT_COVER_ID is null 
       AND p.id=pc_hb.ENDORSMENT_ID AND tplot_hb.external_code = 'HB' AND pc_hb.COVER_STATUS_ID = 20) = 1
       --Avoid where a PFR has existed for this term as these are likely to be unresolved in NPE.
  and (select count(0) from p_policy p1 
       where p1.policy_header_id = p.policy_header_id 
	   and p1.status_id = 10 
	   and p1.status_date between DATEADD(day, -31, p1.policy_start_date) and convert(date, GETDATE())) = 0
ORDER BY newid();