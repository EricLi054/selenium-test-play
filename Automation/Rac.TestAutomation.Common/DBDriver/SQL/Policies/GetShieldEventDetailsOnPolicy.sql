SELECT  u.name_of_user
        ,ER.LOGGED_IN_B2C_CONTACT_NAME
        ,TET.EVENT_TYPE_DSC
FROM    E_EVENT E
JOIN    E_EVENT_RACI ER     ON  ER.EVENT_NR = E.EVENT_NR
JOIN    p_pol_header ph     ON  ph.active_policy_id = e.policy
JOIN    p_policy p          ON  p.id = ph.active_policy_id
JOIN    t_user u            ON  u.userid = e.INSERT_USER
JOIN    T_EVENT_TYPE TET    ON  TET.EVENT_TYPE = E.EVENT_TYPE
WHERE   p.external_policy_number = @policynumber
AND     E.INSERT_USER in (5000219,5000725,5000726,70000565)
AND     E.EVENT_TYPE = 728 --General change
AND     ER.LOGGED_IN_B2C_CONTACT_NAME IS NOT NULL