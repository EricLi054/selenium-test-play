/*
PURPOSE: Find contact attached to a specified motor policy
    as a driver_id and filtered by a provided first name.
AUTHOR:  Unknown
LAST UPDATE: Troy Hall 2024-11-18
*/
SELECT tit.description as Title,
       c.first_name as FirstName, 
       c.name AS Surname, 
	   FORMAT(cp.DATE_OF_BIRTH, 'dd/MM/yyyy') as DateOfBirth, 
       case when (select count(*) from p_policy_contact ppc 
	              where ppc.policy_id = PH.ACTIVE_POLICY_ID 
				  and ppc.contact_id = c.id
				  and ppc.POLICY_CONTACT_ROLE != 1000002)>0 then 1  --Not an account holder role.
		    else 0 end isPH, 
       ca.house_nr as HouseNumber, 
	   ca.street_name AS Street, 
	   ca.city_name AS Suburb, 
	   ca.zip AS Postcode,
	   cntm.telephone_number as MobilePhoneNumber,
	   cnth.telephone_number as HomePhoneNumber
FROM p_policy p 
JOIN p_pol_header ph ON ph.active_policy_id = p.id 
join p_cover  pc on ph.active_policy_id=pc.ENDORSMENT_ID 
join t_product_line_option tplo on pc.product_option_id = tplo.id 
join T_PRODUCT_LINE_OPTION_TYPE tplot on tplot.id = tplo.option_type_id 
join P_POLICY_LOB_ASSET loba on pc.asset_id=loba.id 
join as_asset aas on loba.lob_asset_id=aas.id 
join as_vehicle_drivers drivers on drivers.asset_id=aas.id 
JOIN cn_contact c ON drivers.driver_id = c.id 
JOIN cn_person cp ON c.id = cp.CONTACT_ID 
JOIN t_title tit ON tit.id = cp.title
JOIN CN_Contact_address cca ON cca.contact_id = c.id 
JOIN cn_address ca ON Cca.adress_ID = CA.id 
LEFT JOIN cn_contact_telephone cntm on cntm.contact_id = c.id and cntm.TELEPHONE_TYPE = 4 and cntm.discontinue_date is null
LEFT JOIN cn_contact_telephone cnth on cnth.contact_id = c.id and cnth.TELEPHONE_TYPE = 3 and cnth.discontinue_date is null
WHERE p.external_policy_number = @policynumber 
  AND cca.Address_type = 2 -- 2 Mailing Address
  AND cca.discontinue_date IS NULL 
  AND c.first_name = @firstname 
  AND tplot.external_code in ('MFCO','MTFT','MTPO')
