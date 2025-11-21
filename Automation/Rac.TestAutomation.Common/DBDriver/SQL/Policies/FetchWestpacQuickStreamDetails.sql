/*
PURPOSE: This query fetches the WestpacQuickStream response details against the policy number

AUTHOR: Kris Raymundo
LAST UPDATE: Kris Raymundo 2021-08-05
*/
SELECT STATUS as PaymentStatus
      ,AMOUNT as Amount
      ,RECEIPT_NUMBER as B2CReceiptNumber
      ,RESPONSE_CODE as ResponseCode
      ,RESPONSE_DESCRIPTION as ResponseDescription
      ,CARD_HOLDER as CardHolder
      ,ROADSIDE_PRODUCT as RoadsideProduct
      ,ROADSIDE_AMOUNT as RoadsideAmount
      ,IS_QUICKSTREAM_API as IsQuickStreamApi
      ,WSTPC_CUSTMR_REF_NR as WestpacCustomerRefNumber
      ,WSTPC_HTTP_STATUS as WestpacHttpStatus
FROM AC_SECURE_CARD_PAYMENT_RACI
WHERE CUSTOMER_REFERENCE_NUMBER = @policynumber
AND UPDATE_DATE > convert(datetime, DATEADD(minute, -10, GETDATE()))
ORDER BY UPDATE_DATE DESC;