/*
PURPOSE: Fetch policy and endorsement details after renewal.

AUTHOR: Unknown
LAST UPDATED: Jason King 2020-09-10
*/
SELECT p.EXTERNAL_POLICY_NUMBER,
    p.id,
    PH.MAIN_RENEWAL_DATE,
    p.ENDORS_START_DATE,
    p.ENDORSMENT_TYPE_ID as E_ID -- 1=General Changes (For when a payment is for processing?)
    ,FORMAT(p.policy_end_date, 'yyyy-MM-dd hh:mm:ss') AS Term_end
    ,FORMAT(e.event_date, 'dd/MM/yyyy') AS Event_Date
        ,tet.EVENT_TYPE_DSC AS Event_Type
    ,tdt.dsc AS Event_Document
    ,FORMAT(p.renewal_date, 'yyyy-MM-dd hh:mm:ss') AS Renew_Date
    ,p.yearly_premium AS Ignore_Premium

  FROM p_pol_header ph
  JOIN p_policy p               ON p.id = ph.active_policy_id
  JOIN e_event e                ON e.policy = p.id
  JOIN t_event_type tet         ON tet.event_type = e.event_type
  JOIN e_event_document ed      ON ed.event_id = e.event_nr
  JOIN t_document_type tdt      on tdt.doc_type_nr = ed.document_type
  WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber
  and   e.event_type = 2000136 -- Policy Endorsement Certificate Print
;