/*
PURPOSE: Find a Shield User with Master Role related to a Shield Contact Id.
AUTHOR:  Troy Hall
LAST UPDATE: 2025-05-27 Troy Hall
*/

select 
    tu.name_of_user as username
    ,tur.role_id as user_role_id
    ,tr.role_dsc as role_description 
from t_user tu 
    join t_user_role tur on tur.userid = tu.userid
    join t_role tr on tr.role_id = tur.role_id
where 
    tur.role_id = 200 -- Master role
    and tu.contact_id = @contactId
;