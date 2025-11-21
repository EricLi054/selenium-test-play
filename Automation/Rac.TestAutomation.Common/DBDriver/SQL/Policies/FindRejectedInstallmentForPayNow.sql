/*
PURPOSE: Find Annual Policy that has an instalment that was Rejected after the DUE_DATE.

AUTHOR: Blake Laroux(See https://rac-wa.atlassian.net/wiki/spaces/SS/pages/2943943702/Active+policies+with+Rejected+Instalment)
LAST UPDATED: Troy Hall 2025-04-15

*/
SELECT  pp.external_policy_number as Policy_Number
        ,tcm1.description as policy_collection_method
        ,tpt.description as policy_Payment_frequency
FROM         ac_installment aci
        JOIN p_policy pp                      ON  pp.id = aci.policy_id
        JOIN p_pol_header ph                  ON  ph.id = pp.POLICY_HEADER_ID
        JOIN t_installment_status_type tist   ON  tist.id = aci.installment_status
		JOIN t_collection_method tcm          ON  tcm.id = aci.COLLECTION_METHOD_ID
		JOIN t_collection_method tcm1         ON  tcm1.id = pp.COLLECTION_METHOD_ID
        JOIN t_payment_terms tpt              ON  tpt.id = pp.payment_term_id
		JOIN T_INSTALLMENT_ORIGIN tio		  ON  tio.ID = aci.INSTALLMENT_ORIGIN_ID
WHERE   1=1
AND     aci.reject_number > 0 --Change to return instalments with just 1 or 2 rejection count
AND     aci.installment_status = 5 --Instalment Status in (Rejected, Submitted, Partially Paid)
AND     convert(date, aci.DUE_DATE) < convert(date, aci.UPDATE_DATE)
AND     ph.status_id = 20 --Active policies
AND     pp.COLLECTION_METHOD_ID = @collectionMethod -- 4	Credit card, 1	Cash, 2	Direct Debit, 1000000	Cheque,1000001	Transfer
AND     tpt.description = 'Yearly'     -- 'Semi-Annual', 'Yearly', 'Monthly'
AND     ph.product_id = @productid --1000000 Motor, 1000001	Home, 1000032Motor Cycle,1000007 Electric Mobility,1000033 Boat,1000008	Caravan/ Trailer,4000000 Pet
ORDER BY aci.COLLECTION_DATE DESC;