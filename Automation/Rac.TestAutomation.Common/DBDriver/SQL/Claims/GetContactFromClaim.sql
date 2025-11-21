/*
PURPOSE: Return the claim contacts for a given role and
         claimant side (insured or third party).
AUTHOR:  unknown
LAST UPDATE: Jason King 2022-07-15
*/
SELECT  c.first_name      as "TPFirstName"
        ,c.name           as "TPLastName"
        ,tccr.description as "TPRole"
        ,t.description    as "TPEmailType"
        ,ttt.description  as "TPPhoneType"
        ,tat.description  as "TPAddressType"
        ,ca.street_name   as "TPAddressStreetName"
        ,ca.city_name     as "TPAddressCityName"
FROM          C_claim cl                        
        JOIN  C_CLAIM_CONTACT CC              ON  Cl.ID = CC.CLAIM_ID
        JOIN  cn_contact c                    ON  cc.contact_id = c.id
        FULL OUTER JOIN  CN_Contact_address cca ON  cca.contact_id = c.id
        JOIN  cn_address ca                   ON  Cca.adress_ID = CA.id
        JOIN  t_address_type tat              ON  tat.id = cca.address_type
        JOIN  CN_CONTACT_EMAIL ce             ON  c.id = ce.contact_id
        JOIN  cn_contact_telephone cct        ON  c.id = cct.contact_id
        JOIN  t_email_type t                  ON  ce.email_type = t.id
        JOIN  t_telephone_type ttt            ON  cct.telephone_type = ttt.id
        JOIN  t_claim_contact_role tccr       ON tccr.id = cc.role_id
        /* If the contact requested was the driver-not-claimant (9) or witness (2)
         * then we link the search on claimant side.
         * Otherwise it is a third party or other and they are just linked to
         * claimant in general.
         */
WHERE   (SELECT CASE  --Claimant Side: 1 Policyholder, 2 Third Pary, 3 Insured Claimant.
                      --Related Entity: 436 claim contact, 405 claim asset, 435 and others appear to only relate to claim.
		              WHEN cc.RELATED_ENTITY_NR=436 THEN (SELECT TOP 1 cc_side.ID FROM C_CLAIM_CONTACT cc_side JOIN C_CLAIMANT ct_side on cc_side.ID = ct_side.ID 
					                                      WHERE cc_side.CLAIM_ID = Cl.ID AND ct_side.CLAIMANT_SIDE_ID = @claimantSideID)
					  WHEN cc.RELATED_ENTITY_NR=405 THEN (SELECT TOP 1 ccass.ID FROM C_CLAIMANT_ASSET ccass JOIN C_CLAIMANT ct_side on ccass.CLAIMANT_ID = ct_side.id JOIN C_CLAIM_CONTACT cc_side ON cc_side.ID = ct_side.ID
					                                      WHERE cc_side.CLAIM_ID = Cl.ID AND ct_side.CLAIMANT_SIDE_ID = @claimantSideID)
					  ELSE (SELECT TOP 1 cc.RELATED_ENTITY_ID FROM C_CLAIMANT ct_side
					         WHERE cc.ID = ct_side.id AND ct_side.CLAIMANT_SIDE_ID = @claimantSideID)
			     END) = cc.RELATED_ENTITY_ID 
        AND     cc.ROLE_ID = @roleID -- Claim Contact role: 1 Claimant, 2 Witness, 6 PolicyHolder, 9 Driver, 1000017 Third Party
        AND     ce.DISCONTINUE_DATE IS NULL
        AND     cct.DISCONTINUE_DATE IS NULL
        AND     cca.Address_type = 2 --Mailing Address
        AND     cca.discontinue_date IS NULL
        AND     cl.claim_number = @claimNumber
;
