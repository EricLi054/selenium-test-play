/*
PURPOSE: Fetch all correspondence for claim number within the last 24 hours.

AUTHOR: Hashili
LAST UPDATED: 2025-06-20
*/

SELECT 	cpf.FILE_NAME,
		(select T_DOCUMENT_TYPE.DSC from T_DOCUMENT_TYPE where T_DOCUMENT_TYPE.DOC_TYPE_NR= cc.DOCUMENT_TYPE) as DOC_TYPE,
		(select tft.DESCRIPTION from T_FILE_TYPE tft where tft.ID = cpf.FILE_TYPE) as FILE_TYPE,
		cc.CREATION_DATE,
		(select IS_ACTIONABLE from COM_COMMUNICATION_RACI where ID = cpf.COMM_ID) as ISACTIONABLE,
		cc.REMARKS
FROM   COM_PHYSICAL_FILE cpf
JOIN   COM_COMMUNICATION cc on cc.ID = cpf.COMM_ID
JOIN   COM_REFFERAL_DATA crd on crd.COMM_ID = cc.ID and crd.ENTITY_ID = @claimNumber
WHERE  cpf.UPDATE_DATE > DATEADD(day, -1, convert(date, GETDATE()))