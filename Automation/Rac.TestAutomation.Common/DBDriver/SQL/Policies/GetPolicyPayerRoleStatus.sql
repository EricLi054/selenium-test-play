/*
PURPOSE: This query fetches the payer for the linked bank account
         and returns whether that account holder is a policyholder
         or not. Also returns an indicator for if the policy is
         paid Annual Cash as those will not have a linked bank
         account.

         NOTE: There are quite a few items returned in the SELECT
         and this is support debugging if issues occur with this
         query. This query is expected to be usable for any
         policy regardless of how they are paid.

AUTHOR: Jason King
LAST UPDATE: Jason King 2024-12-27
*/
SELECT p.POLICY_OWNER_BANK_ACCOUNT_ID
	  ,ccba.CONTACT_ID
	  ,(select count (*) from p_policy_contact ppc2 where ppc2.policy_id = ph.ACTIVE_POLICY_ID AND ppc2.CONTACT_ID = ccba.CONTACT_ID AND ppc2.POLICY_CONTACT_ROLE = 6) as PH
	  ,(select count (*) from p_policy_contact ppc2 where ppc2.policy_id = ph.ACTIVE_POLICY_ID AND ppc2.CONTACT_ID = ccba.CONTACT_ID AND ppc2.POLICY_CONTACT_ROLE = 8) as coPH
	  ,(select count (*) from p_policy_contact ppc2 where ppc2.policy_id = ph.ACTIVE_POLICY_ID AND ppc2.CONTACT_ID = ccba.CONTACT_ID AND ppc2.POLICY_CONTACT_ROLE = 1000000) as AuthParty
	  ,(select count (*) from p_policy_contact ppc2 where ppc2.policy_id = ph.ACTIVE_POLICY_ID AND ppc2.CONTACT_ID = ccba.CONTACT_ID AND ppc2.POLICY_CONTACT_ROLE = 1000002) as AccHolder
	  ,(select count (*) from p_policy_contact ppc2 where ppc2.policy_id = ph.ACTIVE_POLICY_ID AND ppc2.CONTACT_ID = ccba.CONTACT_ID AND ppc2.POLICY_CONTACT_ROLE = 1000003) as Disclosure
	  ,CASE WHEN (select count (*) from p_policy_contact ppc2 where ppc2.policy_id = ph.ACTIVE_POLICY_ID AND ppc2.CONTACT_ID = ccba.CONTACT_ID AND ppc2.POLICY_CONTACT_ROLE in(6,8))>0
	        THEN 1
			ELSE 0
			END IsPayeePolicyholder
	  ,CASE WHEN p.COLLECTION_METHOD_ID = 1 THEN 1 ELSE 0 END IsAnnualCash

  FROM p_policy p
  JOIN p_pol_header ph              ON p.id = ph.active_policy_id
  LEFT JOIN cn_contact_bank_account ccba ON p.POLICY_OWNER_BANK_ACCOUNT_ID = ccba.id
 WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber;