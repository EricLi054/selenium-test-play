/*
PURPOSE: Fetch Caravan Policy data for verification
--AUTHOR: Troy Hall
LAST UPDATE: Troy Hall 2021-06-25
*/
select 
/*
Fetch Contact information
*/
    cn.id                        as contact_id
    ,tti.description             as title      
    ,cn.first_name               as first_name
    ,cn.middle_name              as middle_name
    ,cn.name                     as surname 
    ,cnp.date_of_birth           as date_of_birth
    ,tcns.description            as tenure 
    ,SUBSTRING(cni.id_value,1,9) as member_number 
    ,cnadd.house_nr              as house_number
    ,cnadd.street_name           as street_name
    ,cnadd.city_name             as suburb
    ,cnadd.zip                   as post_code 
    ,tg.description              as gender 
    ,cne.email                   as email 
    ,cne.email_type              as email_type
    ,tet.description             as email_type_trans 
/*
Fetch Policy information
*/  
    ,p.external_policy_number                       as policy_number
    ,format(p.policy_start_date,'dd/MM/yyyy HH:mm') as policy_start_date
    ,format(ph.policy_end_date,'dd/MM/yyyy HH:mm')  as policy_end_date 
    ,format(p.premium_for_coll,'N2')                as premium_for_collection
    ,asvr.NCB_LEVEL_ID                              as No_Claims_Bonus_Level_ID
    ,tblr.description                               as NCB_Description
    ,tlp.description                                as Garaged_Value 
    ,upper(asi.value)                               as Registration_Val
    ,tp.id                                          as campaign_code 
    ,tp.description                                 as campaign_code_description
    ,asv.model_dsc                                  as model_description
    ,tplo.external_code                             as Cover_Type_ID
    ,tplo.description                               as Cover_Type_Description  
/*
Fetch Accounting information
*/      
    ,ai.installment_number            as installment_number
    ,format(ai.estimated_amount,'N2') as instalment_amount 
    ,ai.installment_type              as installment_type
    ,tit.description                  as installment_type_trans 
    ,ai.payment_terms_id              as instalment_payment_terms 
    ,ai.collection_method_id          as instalment_collection_method 
    ,tpt1.description                 as acct_payment_terms_trans 
    ,tcm1.description                 as acct_collect_method_trans 
    ,p.payment_term_id                as policy_payment_terms 
    ,p.collection_method_id           as policy_collection_method
    ,case when tpt.description = 'Yearly' then 'Annual' 
	      else tpt.description end       pol_payment_terms_trans 
    ,tcm.description                  as pol_collect_method_trans 
from      p_policy p
     join p_pol_header ph                      on ph.active_policy_id = p.id
     join p_policy_contact ppc                 on ppc.policy_id = p.id
     join cn_contact cn                        on cn.id = ppc.contact_id
     join ac_installment ai                    on ai.policy_header_id = ph.id
     join t_installment_type tit               on tit.id = ai.installment_type
     join t_payment_terms tpt                  on tpt.id = p.payment_term_id 
     join t_payment_terms tpt1                 on tpt1.id = ai.payment_terms_id 
     join t_collection_method tcm              on tcm.id = p.collection_method_id 
     join t_collection_method tcm1             on tcm1.id = ai.collection_method_id 
     join cn_person cnp                        on cnp.contact_id = ppc.contact_id
     join cn_contact_address cna               on cna.contact_id = ppc.contact_id
     join cn_address cnadd                     on cnadd.id = cna.adress_id
     join t_title tti                          on tti.id = cnp.title
     join t_gender tg                          on tg.id = cnp.gender    
     join cn_contact_email cne                 on cne.contact_id = cn.id
     join t_email_type tet                     on tet.id = cne.email_type
     join  p_policy_lob ppl                    on ph.active_policy_id = ppl.policy_id
     join  p_policy_lob_to_lob_asset ppltla    on ppl.id = ppltla.policy_lob_id
     join  p_policy_lob_asset ppla             on ppltla.lob_asset_id = ppla.id
     join  as_asset ass                        on ppla.lob_asset_id = ass.id
     join  as_asset_identifier asi             on asi.asset_id = ass.id
     join  as_vehicle asv                      on ass.id = asv.asset_id
     join  as_vehicle_raci asvr                on asvr.id = asv.id
     left join  t_bm_level_raci tblr           on tblr.id = asvr.ncb_level_id
     join  t_locked_parking tlp                on tlp.id = asv.park_id
     join  p_cover pc                          on ph.active_policy_id = pc.endorsment_id
     join t_product_line_option tplo           on tplo.id = pc.product_option_id
     join t_product_promotion tpp              on  tpp.product_id = ph.product_id 
     join t_promotion tp                       on tp.id = tpp.promotion_id
     full outer join cn_contact_status cns     on cns.contact_id = ppc.contact_id
												    and cns.discontinue_date is null --No discontinued Contact Statuses
     full outer join t_contact_status tcns     on tcns.id = cns.status_id
     full outer join cn_contact_identifier cni on cni.contact_id = ppc.contact_id     
													and cni.id_type = 1 --Only return Contact Identifiers that are RACWA Membership type.
													and cni.discontinue_date is null --No discontinued RACWA Memberships

where   p.external_policy_number = @policynumber  --For existing buffered Policy Number
    and tplo.external_code in ('ACAO','AOCO') --Caravan On-Site & Caravan Contents
    and tplo.description not like 'Aussie Assist%'
    and ai.installment_number = 1 --First instalment only
    and cna.discontinue_date is null --No discontinued Addresses
    and cne.discontinue_date is null --No discontinued Emails
    and cnp.discontinue_date is null --No discontinued Contact Person records
    and cne.email_type = 1 --Only return email of the Private type
order by tplo.external_code --Ordering by Product Line of Business External Code for predictable table results
