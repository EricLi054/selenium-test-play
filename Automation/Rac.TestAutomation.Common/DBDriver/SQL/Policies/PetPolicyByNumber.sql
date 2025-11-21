/*
PURPOSE: Fetch created pet policy to verify details.

AUTHOR: Unknown
LAST UPDATED: Jason King 2020-09-11
*/
select 
	p.external_policy_number
	,ppc.contact_id
    ,(select cp.date_of_birth from cn_person cp where cp.contact_id = ppc.contact_id) as owner_date_of_birth
    ,p.collection_method_id
    ,(select tcm.description from t_collection_method tcm where tcm.id = p.collection_method_id) as collection_method_dsc
    ,(select case when tpt.description = 'Yearly' then 'Annual' 
		          else tpt.description end paymentFrequency 
        from t_payment_terms tpt where tpt.id = p.payment_term_id) as payment_terms_dsc
	,p.yearly_premium
	,asspet.pet_owner_rated_age
	,asspet.pet_name
    ,tpettype.description as pet_type
	,tpetbreed.description as pet_breed
	,FORMAT(p.policy_start_date,'dd/MM/yyyy')
	,SUBSTRING(tplo.description,1,18) as prod_line_option
    ,cast((select count(ai1.installment_number) from ac_installment ai1 where p.id = ai1.policy_id) as CHAR) as instalment_count
	,asspet.IS_PRE_EXIST_ILLNESS
from 
	p_policy p
	join p_pol_header ph                on ph.active_policy_id = p.id
	join p_policy_contact ppc           on ppc.policy_id = ph.active_policy_id
	join p_cover pc                     on pc.endorsment_id = p.id
	join t_product_line_option tplo     on tplo.id = pc.product_option_id
	join p_policy_lob plob              on plob.policy_id = p.id
	join p_policy_lob_to_lob_asset plla on plla.policy_lob_id=plob.id
	join p_policy_lob_asset plobass     on plobass.id = plla.lob_asset_id
	join as_asset ass                   on ass.id = plobass.lob_asset_id
	join as_pet_raci asspet             on asspet.asset_id = ass.id
	join t_pet_type_raci tpettype       on tpettype.id = asspet.pet_type_id
	join t_pet_breed_raci tpetbreed     on tpetbreed.id = asspet.pet_breed_id
where   p.external_policy_number = @policynumber 
order by tplo.id desc
;