/*
PURPOSE: Find a contact by first and last name. Include role
         which can assist in determining if they're staff.
AUTHOR:  Jason King
LAST UPDATE: Jason King 2021-05-04
*/
SELECT c.id,
       c.first_name,
       c.name,
       FORMAT(cp.date_of_birth,'dd/MM/yyyy'),
	   ccr.ROLE_ID,
	   tcr.description
  from cn_contact c
  JOIN cn_contact_address ca  ON ca.contact_id = c.id
  JOIN cn_person cp           ON cp.contact_id = c.id 
  JOIN cn_contact_role ccr    ON ccr.CONTACT_ID = c.id
  join T_CONTACT_ROLE tcr     on tcr.id = ccr.ROLE_ID
  where c.name = @lastName 
    AND c.first_name = @firstName
    AND cp.date_of_birth is not null
;