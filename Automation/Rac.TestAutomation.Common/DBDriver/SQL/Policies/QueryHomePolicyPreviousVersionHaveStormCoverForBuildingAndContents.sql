/*
PURPOSE: For a given home policy, verifies if the previous version
         had storm cover for building and contents. This is a very
		 specific query to support the test scenario of lodging a
		 Storm claim with an event date that falls prior to the
		 most recent endorsement.

		 Assumes:
		   * Home Policy has Building and contents
		   * Home policy has at least one endorsement prior
		 These assumptions are expected to be fulfilled by the caller.
AUTHOR: Jason King
LAST UPDATE: Jason King 2021-07-15
 */
 SELECT count(*)
  FROM p_policy p
  JOIN p_pol_header ph ON ph.active_policy_id = p.id
  JOIN p_policy p1     ON p1.policy_version_nr =  p.policy_version_nr - 1 AND
	                      p1.external_policy_number = p.external_policy_number
 WHERE  p.external_policy_number = @policynumber
   AND (SELECT count(*) from t_product_line_option tplo join p_cover pc on pc.endorsment_id = p1.id WHERE tplo.id = pc.product_option_id AND tplo.external_code = 'OBST') > 0
   AND (SELECT count(*) from t_product_line_option tplo join p_cover pc on pc.endorsment_id = p1.id WHERE tplo.id = pc.product_option_id AND tplo.external_code = 'OCST') > 0