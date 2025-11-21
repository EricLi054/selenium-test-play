/* Retrieve a contact by their contact ID.
 * Last edited: 2024-08-02 - Troy Hall
 */
SELECT 
  c.id as ContactId,
  (SELECT tt.DESCRIPTION from T_TITLE tt where tt.id = cp.TITLE) AS Title,
  c.first_name,
  c.middle_name,
  c.name AS Surname,
  FORMAT(cp.DATE_OF_BIRTH,'yyyy-MM-dd') as DateOfBirth,
  (SELECT tg.external_code FROM T_GENDER tg WHERE cp.gender = tg.id) AS Gender,
  ca.house_nr as House_nr,
  ca.street_name AS Street,
  ca.city_name   AS Suburb,
  ca.zip         AS Postcode,
  (SELECT top 1 cne.email FROM cn_contact_email cne WHERE cne.email_type = 1 and cne.discontinue_date is null and cne.contact_id = c.id) as PrivateEmail,
  (SELECT top 1 cnt.telephone_number FROM cn_contact_telephone cnt WHERE cnt.telephone_type = 4 and cnt.discontinue_date is null and cnt.contact_id = c.id) as MobilePhone,
  (SELECT top 1 CONCAT(cnt.TELEPHONE_PREFIX, cnt.telephone_number) FROM cn_contact_telephone cnt WHERE cnt.telephone_type = 3 and cnt.discontinue_date is null and cnt.contact_id = c.id) as HomePhone,
  (SELECT top 1 CONCAT(cnt.TELEPHONE_PREFIX, cnt.telephone_number) FROM cn_contact_telephone cnt WHERE cnt.telephone_type = 2 and cnt.discontinue_date is null and cnt.contact_id = c.id) as WorkPhone,
  tcs.external_code as MemberStatus,
  cci.id_value as MemberNo,
  tdt.description as PreferredDeliveryMethod

    FROM cn_contact c
    JOIN cn_person cp               ON c.id = cp.CONTACT_ID  and cp.DISCONTINUE_DATE is null
    JOIN CN_Contact_address cca     ON cca.contact_id = c.id and cca.DISCONTINUE_DATE is null
    JOIN cn_address ca              ON Cca.adress_ID = CA.id
LEFT JOIN cn_contact_status cs      on cs.contact_id = c.id  and cs.DISCONTINUE_DATE is null
LEFT JOIN t_contact_status tcs      on tcs.id = cs.status_id AND tcs.DISCONTINUE_DATE is null
LEFT JOIN cn_contact_identifier cci on cci.contact_id = c.id and cci.id_type = 1 and cci.DISCONTINUE_DATE is null
LEFT JOIN t_delivery_type tdt       on tdt.id = c.PREFERRED_DELIVERY_TYPE_ID

WHERE c.id = @ContactId;