/*
PURPOSE: For a given motor policy, it returns the primary
cover on the previous version. Used to support some claim
scenarios where the claim event date applied to a previous
version.
AUTHOR: Jason King
LAST UPDATE: Jason King - 2024-07-31
*/
SELECT tplo1.external_code as Cover
FROM     p_policy p
    JOIN p_pol_header ph                        ON ph.active_policy_id = p.id
                                                AND p.status_id = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
    JOIN p_policy p1     ON p1.external_policy_number = p.external_policy_number
	                    AND p1.policy_version_nr =  p.policy_version_nr-1
    JOIN p_cover pc1     ON pc1.endorsment_id = p1.id
    JOIN t_product_line_option tplo1 ON tplo1.id = pc1.product_option_id

WHERE    p.external_policy_number = @policynumber
     AND p1.endors_start_date < DATEADD(month, -1, p.endors_start_date)
	 AND pc1.PARENT_COVER_ID is null;