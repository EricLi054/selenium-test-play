/*
PURPOSE: This query fetches recent events description of the policy.

AUTHOR: Jason King
LAST UPDATE: Kris Raymundo 2021-08-05
*/
SELECT  TET.EVENT_TYPE_DSC
        ,FORMAT(e.EVENT_DATE, 'd MMM yyyy') as EVENT_DATE
FROM    E_EVENT E
JOIN    p_pol_header ph     ON  ph.active_policy_id = e.policy
JOIN    p_policy p          ON  p.id = ph.active_policy_id
JOIN    T_EVENT_TYPE TET    ON  TET.EVENT_TYPE = E.EVENT_TYPE
WHERE   p.external_policy_number = @policynumber
AND   e.event_date > convert(datetime, DATEADD(minute, -10, GETDATE()))
ORDER BY e.event_date DESC;