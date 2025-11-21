/* General query to get all policy holders for any
 * insurance policy. Order of contacts returned
 * are not sorted in any priority other than main
 * policyholder first.
 * Last edited: 2021 Sep 20 - Jason King
 */
SELECT (SELECT tt.DESCRIPTION from T_TITLE tt where tt.id = cp.TITLE) AS Title,
  c.first_name AS FirstName,
  c.MIDDLE_NAME AS MiddleName,
  c.name AS Surname,
  FORMAT(cp.DATE_OF_BIRTH,'dd/MM/yyyy') AS DOB,
  tcpr.description AS PolicyContactRole,
  ca.house_nr	 AS HouseNumber,
  ca.street_name AS Street,
  ca.city_name   AS Suburb,
  ca.zip         AS Postcode,
  c.id           AS ContactId,
  c.EXTERNAL_CONTACT_NUMBER AS ContactExternalNumber,
  (SELECT top 1 cne.email FROM cn_contact_email cne WHERE cne.email_type = 1 and cne.discontinue_date is null and cne.contact_id = c.id) as PrivateEmail,
  (SELECT top 1 cnt.telephone_number FROM cn_contact_telephone cnt WHERE cnt.telephone_type = 4 and cnt.discontinue_date is null and cnt.contact_id = c.id) as MobilePhone,
  (SELECT top 1 CONCAT(cnt.TELEPHONE_PREFIX, cnt.telephone_number) FROM cn_contact_telephone cnt WHERE cnt.telephone_type = 3 and cnt.discontinue_date is null and cnt.contact_id = c.id) as HomePhone  
FROM p_policy p
JOIN p_pol_header ph            ON ph.active_policy_id = p.id
JOIN p_policy_contact ppc       ON ppc.policy_id = PH.ACTIVE_POLICY_ID
JOIN cn_contact c               ON ppc.contact_id = c.id
JOIN cn_person cp               ON c.id = cp.CONTACT_ID
JOIN CN_CONTACT_ROLE ccr        ON ccr.CONTACT_ID = c.id
JOIN CN_Contact_address cca     ON cca.contact_id = c.id
JOIN cn_address ca              ON Cca.adress_ID = CA.id
JOIN T_Contact_Role tcr         ON tcr.id = ccr.role_id
JOIN T_CONTACT_POLICY_ROLE tcpr ON tcpr.id = ppc.POLICY_CONTACT_ROLE
WHERE p.external_policy_number = @policynumber
AND PPC.POLICY_CONTACT_ROLE   IN (8,6,1000000) -- 6=Policyholder 8=co-Policyholder 1000000 = Auth party
AND cca.Address_type           = 2   -- 2=Mailing Address
AND cca.discontinue_date      IS NULL
AND ccr.DISCONTINUE_DATE      IS NULL
AND ccr.ROLE_ID               = 16   -- 16=Policyholder (role is same whether primary PH or co-PH)
ORDER BY c.id,
  tcpr.description,
  tcr.description ASC;