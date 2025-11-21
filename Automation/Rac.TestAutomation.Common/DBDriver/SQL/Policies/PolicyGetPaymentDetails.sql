/* General query to get basic payment details for any
 * insurance policy
 * Last edited: 5 Feb 2021 - Jason King
 */
SELECT tcm.description,
       case when tpt.description = 'Yearly' then 'Annual' 
		    else tpt.description end paymentFrequency, 
	   tpt.number_of_payments
FROM p_policy p
JOIN p_pol_header ph            ON ph.active_policy_id = p.id
JOIN t_collection_method tcm    ON tcm.id = p.COLLECTION_METHOD_ID
JOIN t_payment_terms tpt        ON tpt.id = p.payment_term_id
WHERE p.external_policy_number = @policynumber
