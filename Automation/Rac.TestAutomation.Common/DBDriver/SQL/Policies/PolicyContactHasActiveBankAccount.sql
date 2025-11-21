/*--------------------------------------------------------------------|
| PURPOSE: Check whether the policy contact has at least one bank     |
|          account which is actively paying for a policy recorded     |
|          against that contact.                                      |
|                                                                     |
| AUTHOR: Troy Hall                                                   |
| LAST UPDATE: Troy Hall 2023-11-24                                   |
|--------------------------------------------------------------------*/
select 
	cn.id as contactId
	,p.external_policy_number as policyNumber
	,tpr.description as contactRole
	,tplot.description as policyType
	,ccbar.bsb_number as bsbNumber
	,RIGHT(ccba.BANK_ACCOUNT_EXTERNAL_NUMBER, 3) as account_number_suffix
	,ai.COLLECTION_METHOD_ID as collectionMethodId
from 
	p_policy p 
	 join p_pol_header ph on ph.active_policy_id = p.id
	 join p_policy_contact ppc on ppc.policy_id = p.id 
	 join cn_contact cn on cn.id = ppc.contact_id
	 join t_contact_policy_role tpr on tpr.id = ppc.policy_contact_role
	 join p_cover pc ON p.id = pc.ENDORSMENT_ID
	 join t_product_line_option tplo ON pc.product_option_id = tplo.id
	 join T_PRODUCT_LINE_OPTION_TYPE tplot ON tplot.id = tplo.option_type_id
	 join ac_installment ai on ai.policy_id = p.id 
	 join cn_contact_bank_account ccba on ccba.contact_id = cn.id and ccba.id = ai.contact_bank_account_id
	 join cn_contact_bank_account_raci ccbar on ccbar.id = ccba.id
where 1=1
	and ppc.contact_id = @contactId
	and p.status_id = 20
	and ai.collection_method_id = 2 -- 2 = Direct Debit via Bank Account | 4 = Direct Debit via Credit card | 1 = Cash
	and pc.parent_cover_id is null
group by ccba.bank_account_external_number, ccbar.bsb_number, p.external_policy_number, tplot.description, tpr.description, cn.id, ai.collection_method_id