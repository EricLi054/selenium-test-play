/*
PURPOSE: Find all Shield users by prefix. Useful for getting
         all possible test automation Shield users.
AUTHOR:  Jason King
LAST UPDATE: Jason King 2021-07-04
*/
SELECT tu.NAME_OF_USER
  from t_user tu 
  join t_contact_role tcr on tcr.id = tu.CONTACT_ROLE_ID
 where tu.NAME_OF_USER like @usernamePrefix 
   and tcr.EXTERNAL_CODE = 'StaffMember';