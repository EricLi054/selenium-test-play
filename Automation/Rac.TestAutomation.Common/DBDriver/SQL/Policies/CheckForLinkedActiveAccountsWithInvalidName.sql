/*
PURPOSE: Check whether this contact should be disqualified as a candidate due to associated 
active bank account that have account names that are invalid

AUTHOR: James Barton
LAST UPDATED: James Barton 2022-12-07
*/
SELECT	c.ID, ccbar.CONTACT_BANK_ACCOUNT_NAME
FROM    CN_CONTACT c
JOIN    CN_CONTACT_BANK_ACCOUNT ccba         ON c.id = ccba.CONTACT_ID
JOIN    CN_CONTACT_BANK_ACCOUNT_RACI ccbar   ON ccba.id = ccbar.id
WHERE   ccba.DISCONTINUE_DATE is null -- Active Account record
AND     (ccbar.CONTACT_BANK_ACCOUNT_NAME is null
        OR LEN(ccbar.CONTACT_BANK_ACCOUNT_NAME) < 1
        OR LEN(ccbar.CONTACT_BANK_ACCOUNT_NAME) > 40
        OR ccbar.CONTACT_BANK_ACCOUNT_NAME like '%[1-9()?<>:;"{}|+=*^$#@!~,./]%') 
AND ccba.type = 1         -- Bank Account 
AND c.ID = @contactId