/*
PURPOSE: Find a contact with no Mailing Preferences set for testing setting a preference via PCM.
AUTHOR:  Unknown
LAST UPDATE: Troy Hall 2024-08-02
*/
SELECT TOP 100 c.id as ContactId,
       c.EXTERNAL_CONTACT_NUMBER as ExternalContactNumber,
       FORMAT(cp.date_of_birth,'dd/MM/yyyy') as DateOfBirth,
       p.EXTERNAL_POLICY_NUMBER as PolicyNumber,
       c.first_name,
       c.middle_name,
       c.name,
       case when tdt.description is null then 'null' 
			else tdt.description 
			end as PreferredDeliveryMethod
  from cn_contact c
  JOIN cn_contact_address ca  ON ca.contact_id = c.id
  JOIN cn_address a           ON ca.adress_id = a.id
  JOIN cn_person cp           ON cp.contact_id = c.id 
  JOIN p_policy_contact ppc   ON c.id = ppc.contact_id
  JOIN p_pol_header ph        ON ppc.policy_id = PH.ACTIVE_POLICY_ID and ppc.policy_contact_role in (6,8)
  JOIN p_policy p             ON p.id = ph.active_policy_id
  left JOIN T_DELIVERY_TYPE tdt	  ON tdt.id = c.PREFERRED_DELIVERY_TYPE_ID
  where c.name is not null
    AND c.first_name is not null
    AND cp.date_of_birth is not null
    AND a.street_name is not null
    AND a.ZIP is not null
    AND a.city_name is not null
    AND ph.status_id = 20
	and tdt.description is null
ORDER BY newid();