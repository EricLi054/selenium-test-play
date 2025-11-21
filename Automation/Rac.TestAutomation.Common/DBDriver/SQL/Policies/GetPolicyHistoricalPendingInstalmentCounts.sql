/*
PURPOSE: Fetching count of pending instalments for the given policy
         which preceded a given date. This is because they will trigger
		 irregularities for a claim.
--AUTHOR: Jason King
LAST UPDATE: Jason King 2025-04-22
*/
select sum(pastDueInstallments) as pastDueInstallments,
       sum(pastCollectionInstallments) as pastCollectionInstallments
from
(
  select (select count(*) from ac_installment ai2 
	       where p.ID = ai2.POLICY_ID 
		     and ai2.INSTALLMENT_STATUS = 1 -- 1=Pending
		     and ai2.DUE_DATE < CAST(@datestring as date)
	     ) as pastDueInstallments,
	     (select count(*) from ac_installment ai2 
	       where p.ID = ai2.POLICY_ID 
		     and ai2.INSTALLMENT_STATUS = 1 -- 1=Pending
		     and ai2.COLLECTION_DATE < CAST(@datestring as date)
	     ) as pastCollectionInstallments
   FROM p_policy p
  WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber
) as InstCount
;