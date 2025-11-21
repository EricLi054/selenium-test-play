/*
PURPOSE: Fetch the case event type description for any events related to the TCU Case Number within the last 24 hours.
AUTHOR: Hashili 16-Oct-2025
*/
SELECT TE.EVENT_TYPE_DSC Event_Type 
FROM E_EVENT EE
JOIN E_EVENT_ADDITIONAL_REF EEA		ON EEA.EVENT_NR=EE.EVENT_NR
JOIN T_EVENT_TYPE TE				ON TE.EVENT_TYPE=EE.EVENT_TYPE
where EEA.REF_VALUE = @caseNumber
AND  EE.EVENT_DATE > DATEADD(day, -1, convert(date, GETDATE()))
 