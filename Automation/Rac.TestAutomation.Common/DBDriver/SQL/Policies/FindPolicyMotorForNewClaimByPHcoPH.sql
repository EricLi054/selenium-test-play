/*
PURPOSE: Fetch a list of Full Comprehensive cover motor policies
with an endorsement in the past month.
AUTHOR: Jason King
LAST UPDATE: Jason King - 2024-07-31
*/
SELECT TOP 20
      cn.id                                   AS "ContactId",
      p.external_policy_number                AS "PolicyNumber",
      cn.first_name                           AS "FirstName",
      cn.name                                 AS "LastName",
      format(cp.date_of_birth,'dd/MM/yyyy')   AS "DOB",
      (select tcpr.description FROM t_contact_policy_role tcpr WHERE tcpr.id = ppc.policy_contact_role) AS "ContactPolicyRole",
      format(p.endors_start_date,'dd/MM/yyyy') AS "PolicyEndorsedDate",
	  (SELECT top 1 cnt.telephone_number FROM cn_contact_telephone cnt 
	                                    WHERE cnt.telephone_type = 4 AND cnt.discontinue_date IS NULL AND cnt.contact_id = cn.id)
										      AS mobilePhoneNumber,
	  (SELECT top 1 cne.email            FROM cn_contact_email cne 
	                                    WHERE cne.email_type = 1 AND cne.discontinue_date IS NULL AND cne.contact_id = cn.id)
										      AS privateEmail
FROM     p_policy p
    JOIN p_pol_header ph                        ON ph.active_policy_id = p.id
                                                AND p.status_id = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
												AND ph.product_id = 1000000 -- motor
    JOIN p_policy_contact ppc                   ON p.id = ppc.policy_id     
                                                AND ppc.policy_contact_role IN (6,8)
    JOIN cn_person cp                           ON ppc.contact_id = cp.CONTACT_ID
	JOIN cn_contact cn                          ON cp.contact_id = cn.id
    JOIN p_cover pc                             ON ph.active_policy_id = pc.endorsment_id
    JOIN t_product_line_option tplo             ON tplo.id = pc.product_option_id
	                                            AND tplo.external_code = 'MFCO'
WHERE p.endors_start_date > DATEADD(month, -1, p.policy_start_date)  --Don't try to set event date prior to policy term (31 day period to allow use of the NPE loophole which ignores existing claim checks if Event Date = 01/##/####)
  AND cn.first_name IS NOT NULL --Reduced chance of Deceased Estate being returned
  AND cn.name IS NOT NULL --Reduced chance of Deceased Estate being returned
  AND cp.date_of_birth IS NOT NULL --Humans only please.
ORDER BY newid()
;