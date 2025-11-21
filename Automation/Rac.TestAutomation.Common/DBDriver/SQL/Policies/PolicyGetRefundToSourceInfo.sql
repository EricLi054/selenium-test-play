/*
PURPOSE: For a given policy, check the accounting information to see if it
         supports refund to source for various categories. Will return a "1"
         for the category that is supported. If the policy doesn't have the
         necessary accounting info, then all categories will return "0"
--AUTHOR: Jason King
LAST UPDATE: Jason King 2023-09-13
*/
SELECT 
  ppc.CONTACT_ID,
  p.EXTERNAL_POLICY_NUMBER,
  (select tpv.VERSION_NR from T_PRODUCT_VERSION tpv where tpv.id = p.PRODUCT_VERSION_ID) AS ProdVers,
  act.PAYMENT_CHANNEL_ID,
  (select tpc.DESCRIPTION from T_PAYMENT_CHANNEL tpc where tpc.id = act.PAYMENT_CHANNEL_ID) as PayCh,
  -- Refund to a bank account. 2000017 (RACWA Australia Post), 2000000 (PNP Bpay), 1000027,1000028,1000029,1000030,1000001,1000002,1000003,1000004,1000005,1000006 (EFTPOS)
  case when act.PAYMENT_CHANNEL_ID in (2000017,2000008,2000000,1000027,1000028,1000029,1000030,1000001,1000002,1000003,1000004,1000005,1000006) then 1 else 0 end RefBank,
  -- Refund to an unknown credit card (CC payment was received from a 3rd party portal)
  case when act.PAYMENT_CHANNEL_ID in (1000015,1000017,2000002,2000003,2000004,2000005,2000006,2000007) then 1 else 0 end RefUnknCC,
  -- Refund to a known credit card (CC payment received via RAC systems)
  case when act.PAYMENT_CHANNEL_ID in (1000009,1000010,1000012,1000013,1000014,2000020,2000021,2000022) then 1 else 0 end RefKnwnCC
FROM p_policy p
JOIN p_pol_header ph                  ON ph.active_policy_id = p.id
JOIN p_policy_contact ppc             ON ppc.policy_id = PH.ACTIVE_POLICY_ID

JOIN    (SELECT aa1.POLICY_HEADER_ID, aat1.ACCOUNT_ID, max(act1.ID) as latestId
         FROM   AC_ACCOUNT aa1                  
         JOIN   AC_ACCOUNT_TRANSACTION aat1     ON  aa1.id = aat1.ACCOUNT_ID
         JOIN   AC_TRANSACTION act1             ON  aat1.TRANSACTION_ID = act1.id
         WHERE  1=1
         AND    aa1.ACCOUNT_TYPE = 5 --Policy
         AND    act1.TRANSACTION_TYPE = 1001031 --Receipt
         AND    aat1.IS_PAYMENT = 1
         AND    aa1.POLICY_HEADER_ID is not null
         AND    act1.TRANSACTION_DATE is not null
         GROUP BY aa1.POLICY_HEADER_ID, aat1.ACCOUNT_ID
         ) AS Account_Transaction ON ph.id = Account_Transaction.POLICY_HEADER_ID

LEFT JOIN   AC_ACCOUNT_TRANSACTION aat ON aat.ACCOUNT_ID = Account_Transaction.ACCOUNT_ID
LEFT JOIN   AC_TRANSACTION act         ON aat.TRANSACTION_ID = act.id AND act.id = Account_Transaction.latestId AND act.TRANSACTION_DATE is not null

WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber
    AND act.PAYMENT_CHANNEL_ID is not null
;