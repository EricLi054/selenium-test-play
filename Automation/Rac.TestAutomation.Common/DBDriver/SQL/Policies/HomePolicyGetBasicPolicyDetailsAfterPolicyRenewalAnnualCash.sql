/*
PURPOSE: Fetch renewal endorsement event details.

AUTHOR: Unknown
LAST UPDATED: Jason King 2020-09-10
*/
SELECT p.EXTERNAL_POLICY_NUMBER,
    p.id,
    PH.MAIN_RENEWAL_DATE,
    FORMAT(p.ENDORS_START_DATE, 'dd/MM/yyyy') AS ENDOR_Date,
    p.ENDORSMENT_TYPE_ID as E_ID -- 1=General Changes
    ,p.yearly_premium AS Ignore_Premium
    ,FORMAT(e.event_date, 'dd/MM/yyyy') AS Event_Date
    ,tet.EVENT_TYPE_DSC AS Event_Type
    ,tdt.dsc AS Event_Document
    ,FORMAT(ae.update_date, 'dd/MM/yyyy') AS Update_Date
    ,tes.description as Entry_Status
    ,FORMAT(sum(abs(ae.amount)),'0.00') as tot_amount
    ,ttt.description as TransType
  FROM p_pol_header ph
  JOIN p_policy p               ON p.id = ph.active_policy_id
  JOIN ac_entry ae              ON ae.policy_header_id = ph.id
  JOIN e_event e                ON e.policy = p.id
  JOIN t_event_type tet         ON tet.event_type = e.event_type
  JOIN t_entry_status tes       ON tes.id = ae.entry_status
  JOIN ac_transaction at        ON ae.transaction_id = at.id
  JOIN t_transaction_type ttt   ON at.transaction_type = ttt.id
  JOIN e_event_document ed      ON ed.event_id = e.event_nr
  JOIN t_document_type tdt      on tdt.doc_type_nr = ed.document_type
  WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber
  and   ae.update_date >= convert(date, GETDATE())
  group by p.EXTERNAL_POLICY_NUMBER
    ,p.id
    ,PH.MAIN_RENEWAL_DATE
    ,p.ENDORS_START_DATE
    ,p.ENDORSMENT_TYPE_ID
    ,p.yearly_premium
    ,FORMAT(Event_Date, 'dd/MM/yyyy')
    ,tet.EVENT_TYPE_DSC
    ,tdt.dsc
    ,FORMAT(ae.update_date, 'dd/MM/yyyy')
    ,tes.description
    ,ttt.description
;