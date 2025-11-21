/*
PURPOSE:Retrieve Caravan policies ready for renewal based on payment term & collection method.
AUTHOR: Hashili on 14/03/2024
LAST UPDATED: Lopa on 26/09/2024
*/
SELECT TOP 100 ppc.contact_id,
    p.EXTERNAL_POLICY_NUMBER
  FROM p_policy p
  JOIN p_pol_header ph						ON p.id = ph.active_policy_id
  JOIN P_POLICY_CONTACT ppc				    ON p.id = ppc.POLICY_ID
  JOIN cn_contact c							ON c.id = ppc.contact_id
  JOIN CN_CONTACT_RACI cnr					ON cnr.id = c.id
  join cn_person cnp						ON cnp.contact_id = ppc.contact_id
 WHERE ph.product_id           = 1000008 -- Caravan
  AND p.ENDORS_START_DATE     > convert(date, GETDATE())
  AND ph.status_renewal_date  > DATEADD(day, 1, convert(date, GETDATE()))
  AND PH.MAIN_RENEWAL_DATE    > ph.status_renewal_date
  AND ph.MANUAL_NON_RENEWALS_CODE is null -- This section is to avoid policies that are flagged for manual non-renewal which would put them in a "Proposal For Renewal (PFR)" state on renewal
  AND p.status_ID             = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND ph.status_ID            = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
  AND p.ENDORSMENT_TYPE_ID    = 10 -- 10 Policy Renewal
  AND ppc.policy_contact_role = 6 -- 6=Policyholder 8=co-Policyholder
  AND ph.STATUS_RENEWAL_DATE IS NOT NULL 
  AND p.collection_method_ID  = @collectionmethod -- 2 is Direct Debit by Bank Account | 1 Annual Cash | 4 is Direct Debit by Credit Card
  AND p.payment_term_id       = @paymentterm -- 1 is Yearly Direct Debit | 4 is Monthly Direct Debit | 6 = Yearly Cash | 1000002 = Monthly Credit Card
  AND cnp.date_of_birth is not null -- helps screen out company policies.
  AND (cnr.WESTPAC_CUSTOMER_ID   = c.id or cnr.WESTPAC_CUSTOMER_ID is null) -- Won't be impacted by the issue described in https://rac-wa.atlassian.net/wiki/spaces/SS/pages/2919465010/Westpac+Tokenisation+of+Non-Prod+Environments
ORDER BY newid();