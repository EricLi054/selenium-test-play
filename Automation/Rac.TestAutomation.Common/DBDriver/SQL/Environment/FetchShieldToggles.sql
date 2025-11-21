/*
PURPOSE: Obtain the current state of a toggle for this Shield environment via
         the application parameters table.

AUTHOR: Troy Hall
LAST UPDATED: Troy Hall 2023-09-23
*/
select 
	tap.value,
	tap.id,
	--tap.update_date,					--May be useful for debug/investigation
	--tap.update_version,				--May be useful for debug/investigation
	--tu.name_of_user as update_user,	--May be useful for debug/investigation
	tap.description as param_desc,
	tap.developer_desc as param_dev_desc,
	dat.description as parameter_type,
	dat.developer_desc as parameter_dev_desc
from t_application_params tap 
	join t_dynamic_attribute_type dat on dat.id = tap.parameter_type
	join t_user tu on tu.userid = tap.update_user
where 
tap.description = @toggleReference
;