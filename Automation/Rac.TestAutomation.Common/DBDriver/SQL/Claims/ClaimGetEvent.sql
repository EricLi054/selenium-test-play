/*
PURPOSE: Fetch claim event to confirm that member accepted online settlement.

AUTHOR: Unknown
LAST UPDATED: Jason King 2020-09-10
*/
SELECT te.EVENT_TYPE_DSC Event_Type
FROM c_claim c
JOIN e_event ee                ON ee.claim_nr = c.claim_number
JOIN t_event_type te           ON te.event_type = ee.event_type
WHERE   c.claim_number = @claimNumber;
 