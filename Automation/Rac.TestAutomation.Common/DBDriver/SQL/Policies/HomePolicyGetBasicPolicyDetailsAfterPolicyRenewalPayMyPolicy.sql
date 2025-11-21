SELECT p.EXTERNAL_POLICY_NUMBER,
    p.id,
    PH.MAIN_RENEWAL_DATE,
    p.ENDORS_START_DATE
    ,p.yearly_premium AS Ignore_Premium
    ,FORMAT(ae.update_date, 'dd/MM/yyyy') AS Update_Date
    ,tes.description as Entry_Status
    ,FORMAT(sum(abs(ae.amount)),'0.00') as tot_amount
    ,ttt.description as TransType
  FROM p_pol_header ph
  JOIN p_policy p               ON p.id = ph.active_policy_id
  JOIN ac_entry ae              ON ae.policy_header_id = ph.id
  JOIN t_entry_status tes       ON tes.id = ae.entry_status
  JOIN ac_transaction at        ON ae.transaction_id = at.id
  JOIN t_transaction_type ttt   ON at.transaction_type = ttt.id
  WHERE p.EXTERNAL_POLICY_NUMBER = @policynumber
  and   ae.entry_status = 1 -- 1=paid
  and   ae.update_date >= convert(date, GETDATE())
  and   at.transaction_type NOT IN (1001039) -- Ignore payment request transactions
  group by p.EXTERNAL_POLICY_NUMBER
    ,p.id
    ,PH.MAIN_RENEWAL_DATE
    ,p.ENDORS_START_DATE
    ,p.yearly_premium
    ,FORMAT(ae.update_date, 'dd/MM/yyyy')
    ,tes.description
    ,ttt.description;