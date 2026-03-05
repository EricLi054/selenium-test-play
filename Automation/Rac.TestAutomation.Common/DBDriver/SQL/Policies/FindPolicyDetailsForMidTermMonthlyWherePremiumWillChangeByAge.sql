/*
PURPOSE: Find policies suitable for Update How You Pay where the policyholder's age
is one year before a premium age-threshold (21, 24, 50, 71, 76), so that premium
is expected to change when they cross that age (e.g. at next birthday or renewal).
Used to supply test data for verifying premium change in Update How You Pay tests.

Same base criteria as FindPolicyDetailsForMidTermMonthlyCreditCard/BankDebit
(active, mid-term, monthly payment, only paid/pending instalments, valid phone).
Use parameters to select credit card or bank debit (same keys as GetShieldPaymentMethodParameters):
  Credit card: @collectionmethod=4, @paymentterm=1000002, @isCreditCard=1
  Bank debit:  @collectionmethod=2, @paymentterm=4, @isCreditCard=0

Age thresholds align with Caravan Driver Age Factor (ID 1000220); confirm with
business if Motor/Home use different age bands.

AUTHOR: Automation
LAST UPDATE: 2025-02
*/
SELECT TOP 40
   p.EXTERNAL_POLICY_NUMBER as PolicyNumber,
   c.ID                     as ContactID,
  (SELECT top 1 cnt.telephone_number FROM cn_contact_telephone cnt
    WHERE cnt.telephone_type = 4 AND cnt.discontinue_date is null AND cnt.contact_id = c.id) as MobilePhone,
  (SELECT top 1 CONCAT(cnt.TELEPHONE_PREFIX, cnt.telephone_number) FROM cn_contact_telephone cnt
   WHERE cnt.telephone_type = 3 AND cnt.discontinue_date is null AND cnt.contact_id = c.id) as HomePhone
FROM p_policy p
  JOIN p_pol_header ph                    ON ph.active_policy_id = p.id
  JOIN p_policy_contact ppc               ON ppc.policy_id = PH.ACTIVE_POLICY_ID
  JOIN cn_contact c                       ON c.id = ppc.contact_id
  JOIN cn_person cnp                      ON cnp.contact_id = c.id
  JOIN CN_CONTACT_RACI cnr                ON cnr.id = c.id
  JOIN p_policy_lob ppl                   ON PH.ACTIVE_POLICY_ID = ppl.policy_id
  JOIN cn_contact_bank_account ccba       ON ccba.id = p.POLICY_OWNER_BANK_ACCOUNT_ID
WHERE ph.product_id             = @productId
  AND ph.policy_end_date        between DATEADD(day, 32, convert(date, GETDATE())) and DATEADD(day, 336, convert(date, GETDATE()))
  AND (
    (@isCreditCard = 1 AND (SELECT count(*) FROM CN_CONTACT_BANK_ACCOUNT ccba2 WHERE ccba2.CONTACT_ID = ccba.CONTACT_ID AND ccba2.BANK_NAME in ('AMEX', 'MASTERCARD', 'VISA') and ccba2.DISCONTINUE_DATE is null) >= 1)
    OR
    (@isCreditCard = 0 AND (SELECT count(*) FROM CN_CONTACT_BANK_ACCOUNT ccba2 WHERE ccba2.CONTACT_ID = ccba.CONTACT_ID AND ccba2.BANK_NAME not in ('AMEX', 'MASTERCARD', 'VISA') and ccba2.DISCONTINUE_DATE is null) >= 1)
  )
  AND ph.status_ID              = 20
  AND ppc.policy_contact_role   = 6
  AND C.ENTITY_ID               = 1
  AND ( (@isCreditCard = 1 AND cnr.WESTPAC_CUSTOMER_ID = cnr.ID) OR (@isCreditCard = 0 AND (cnr.WESTPAC_CUSTOMER_ID is null OR cnr.WESTPAC_CUSTOMER_ID = cnr.ID)) )
  AND p.collection_method_ID   = @collectionmethod
  AND p.payment_term_id         = @paymentterm
  AND (SELECT count(ai2.id)
       FROM AC_INSTALLMENT ai2
       WHERE ai2.POLICY_ID = ph.ACTIVE_POLICY_ID
       AND ai2.INSTALLMENT_STATUS NOT in (1,3)) = 0
  AND ((cnr.WESTPAC_CUSTOMER_ID is null) OR (cnr.WESTPAC_CUSTOMER_ID = cnr.ID))
  AND cnp.date_of_birth         is not null
  /*
   * Policyholder age in full years:
   *   DATEDIFF = years between DOB and today.
   *   DATEADD(...) = this year's birthday (same year number as DATEDIFF). If that date > today, birthday not yet reached, so subtract 1.
   * Filter: age IN (20, 23, 49, 70, 75) = one year before premium thresholds (21, 24, 50, 71, 76) for UHIP premium validation.
   */
  AND (DATEDIFF(year, cnp.date_of_birth, GETDATE())
       - CASE WHEN DATEADD(year, DATEDIFF(year, cnp.date_of_birth, GETDATE()), cnp.date_of_birth) > GETDATE() THEN 1 ELSE 0 END)
      IN (20, 23, 49, 70, 75)
ORDER BY newid();
