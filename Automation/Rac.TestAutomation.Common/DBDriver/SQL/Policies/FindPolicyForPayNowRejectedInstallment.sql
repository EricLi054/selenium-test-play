/*
PURPOSE: Find Annual Policy that has an instalment that was Rejected after the DUE_DATE.

AUTHOR: Jason King
LAST UPDATED: Jason King 2025-12-01

*/
SELECT pp.external_policy_number as Policy_Number
        ,tcm1.description as policy_collection_method
        ,tpt.description as policy_Payment_frequency
        ,aci.DUE_DATE
FROM ac_installment aci
        JOIN p_policy pp                      ON  pp.id = aci.policy_id
        JOIN p_pol_header ph                  ON  ph.id = pp.POLICY_HEADER_ID
        JOIN t_installment_status_type tist   ON  tist.id = aci.installment_status
		JOIN t_collection_method tcm          ON  tcm.id = aci.COLLECTION_METHOD_ID
		JOIN t_collection_method tcm1         ON  tcm1.id = pp.COLLECTION_METHOD_ID
        JOIN t_payment_terms tpt              ON  tpt.id = pp.payment_term_id
		JOIN T_INSTALLMENT_ORIGIN tio		  ON  tio.ID = aci.INSTALLMENT_ORIGIN_ID
WHERE   1=1
AND     aci.installment_status in (3) --Instalment Status in (3=Paid,7=Rejected, 6=Submitted, 8=Partially Paid)
AND     convert(date, aci.DUE_DATE) > DATEADD(month, -1, CONVERT(date, GETDATE())) --Instalment
AND     pp.POLICY_VERSION_NR > 1
AND     ph.status_id = 20 --Active policies
AND     pp.COLLECTION_METHOD_ID = @collectionMethod -- 4	Credit card, 1	Cash, 2	Direct Debit, 1000000	Cheque,1000001	Transfer
AND     tpt.description = 'Yearly'     -- 'Semi-Annual', 'Yearly', 'Monthly'
AND     ph.product_id = @productid --1000000 Motor, 1000001	Home, 1000032Motor Cycle,1000007 Electric Mobility,1000033 Boat,1000008	Caravan/ Trailer,4000000 Pet
ORDER BY NEWID();