/*
PURPOSE: Find update contact event in past day.
AUTHOR:  Unknown
LAST UPDATE: Jason King 2020-09-09
*/
SELECT te.EVENT_TYPE_DSC Event_Type,
eer.USER_NAME,
eer.LOGGED_IN_B2C_CONTACT_NAME
from e_event ee
JOIN e_event_raci eer ON eer.event_nr = ee.event_nr
JOIN t_event_type te   ON te.event_type = ee.event_type
WHERE te.EVENT_TYPE_DSC = 'Update contact'
AND eer.USER_NAME = 'B2CTestUser'
AND ee.event_date > DATEADD(day, -1, convert(date, GETDATE()))
AND ee.CLIENT = @ContactId;