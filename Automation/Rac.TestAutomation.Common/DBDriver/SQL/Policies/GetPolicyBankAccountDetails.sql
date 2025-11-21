/*
PURPOSE: Fetch DD banking details from policy.
AUTHOR:  Unknown
LAST UPDATE: Jason King 2020-09-09
*/
SELECT ccbar.BSB_NUMBER,
       ccba.account_nr,
       ccbar.CONTACT_BANK_ACCOUNT_NAME,
       ccba.contact_id
FROM cn_contact_bank_account ccba
JOIN cn_contact_bank_account_raci ccbar ON ccbar.id = ccba.id
JOIN p_policy p                         ON p.POLICY_OWNER_BANK_ACCOUNT_ID = ccba.id
JOIN p_pol_header ph                    ON ph.active_policy_id = p.id
JOIN AC_INSTALLMENT ai                  ON ph.ID = ai.MASTER_POLICY_HEADER_ID
WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber
ORDER BY ai.installment_number DESC;