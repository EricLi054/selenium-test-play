/*
PURPOSE: Find a contact with Roadside Assistance membership.
         Supports up to 5 different member tiers to search for.
         Must have a valid mailing address, private email and mobile number.
AUTHOR:  Unknown
LAST UPDATE: Jason King 19 Aug 2021
*/
SELECT TOP 100  c.id,
         c.EXTERNAL_CONTACT_NUMBER,
         tt.description as title,
         c.first_name,
         c.middle_name,
         c.name as last_name,
         tg.external_code as Gender,
         format(cp.date_of_birth,'dd') as DoBDay,
         format(cp.date_of_birth,'MM') as DoBMonth,
         format(cp.date_of_birth,'yyyy') as DoBYear,
         a.house_nr,
         a.street_name,
         a.city_name,
         a.zip,
         tcs.external_code as MemberStatus,
         cci.id_value as MemberNo,
         (SELECT top 1 cne.email FROM cn_contact_email cne WHERE cne.email_type = 1 and cne.discontinue_date is null and cne.contact_id = c.id) as email,
         (SELECT top 1 cnt.telephone_number FROM cn_contact_telephone cnt WHERE cnt.telephone_type = 4 and cnt.discontinue_date is null and cnt.contact_id = c.id) as MobilePhone
    FROM cn_contact c
    JOIN cn_person cp              on cp.contact_id = c.id
    JOIN T_TITLE tt                on cp.title = tt.id
    JOIN T_GENDER tg               on cp.gender = tg.id
    JOIN cn_contact_address ca     on ca.contact_id = c.id
    JOIN cn_address a              on ca.adress_id = a.id
    JOIN cn_contact_raci cnr       on cnr.id = c.id
    LEFT JOIN cn_contact_status cs      on cs.contact_id = c.id
    LEFT JOIN t_contact_status tcs      on tcs.id = cs.status_id
    LEFT JOIN cn_contact_identifier cci on cci.contact_id = c.id and cci.id_type = 1
    LEFT JOIN cn_contact_id_raci cir    on cir.id = cci.table_id
    WHERE lower(c.name) not like '$_%' escape '$'
      AND a.street_name is not null
      AND a.house_nr is not null
      AND a.city_name is not null
      AND a.province_name is not null
      AND cp.date_of_birth between DateFromParts(@DoBFromYear,@DoBFromMonth,@DoBFromDay) and DateFromParts(@DoBToYear,@DoBToMonth,@DoBToDay)
      AND cp.gender in (@Gender1,@Gender2)
      AND cp.title is not null
      AND c.first_name is not null
      AND c.name is not null
      AND ca.discontinue_date is null
      AND cs.DISCONTINUE_DATE is null
      AND cci.DISCONTINUE_DATE is null
      AND cp.DISCONTINUE_DATE is null
      AND tcs.DISCONTINUE_DATE is null
      AND cnr.DISCONTINUED != 1
      AND 0 < (SELECT COUNT(*) FROM cn_contact_email WHERE email_type = 1 and discontinue_date is null and contact_id = c.id) -- as at least one private email
      AND 0 < (SELECT COUNT(*) FROM cn_contact_telephone WHERE telephone_type in (4) and discontinue_date is null and contact_id = c.id) -- must have a mobile number
      AND ((@IncludeNoMembership = '1' AND tcs.external_code is null)
        OR (tcs.external_code in (@membershipTier1,@membershipTier2,@membershipTier3,@membershipTier4,@membershipTier5)))
ORDER BY newid() 
OPTION(RECOMPILE)