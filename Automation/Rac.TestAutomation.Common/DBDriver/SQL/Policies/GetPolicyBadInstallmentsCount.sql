/*
PURPOSE: Fetching count of undesirable instalments for the given policy
--AUTHOR: Jason King
LAST UPDATE: Jason King 2021-07-20
*/
select (select count(*) from ac_installment ai2 
	     where ph.ID = ai2.MASTER_POLICY_HEADER_ID 
		   and ai2.INSTALLMENT_STATUS = 1 -- 1=Pending
		   and ai2.COLLECTION_METHOD_ID = 3000000 -- OnHold
	   ) as onhold_installments,
       (select count(*) from ac_installment ai2 
	     where ph.ID = ai2.MASTER_POLICY_HEADER_ID 
		   and ai2.INSTALLMENT_STATUS in (5,11) -- 5=Rejected 11=Account Blocked
	   ) as bad_installments,
       (select count(*) from ac_installment ai2 
	     where ph.ID = ai2.MASTER_POLICY_HEADER_ID 
		   and ai2.INSTALLMENT_STATUS = 7 -- 7=Booked
	   ) as booked_installments
  FROM p_policy p
  JOIN p_pol_header ph           ON p.id = ph.active_policy_id
WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber
;
