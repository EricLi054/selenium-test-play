/*
PURPOSE: Fetching count on submitted instalments for the given policy
--AUTHOR: Jason King
LAST UPDATE: Jason King 2021-07-20
*/
select (select count(*) from ac_installment ai2 
	     where ph.ID = ai2.MASTER_POLICY_HEADER_ID 
		   and ai2.INSTALLMENT_STATUS = 6 -- 6=Submitted
	   ) as submitted_installments
  FROM p_policy p
  JOIN p_pol_header ph           ON p.id = ph.active_policy_id
WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber
;
