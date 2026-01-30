/*
PURPOSE: Find current policies for a vehicle (motor/caravan/motorcycle)
     to support tests such as duplicate policy scenarios.
AUTHOR: Eric Li
LAST UPDATE: 2025-12-19  Jason King
*/
SELECT TOP 50
    c.ID AS contact_id,
    p.EXTERNAL_POLICY_NUMBER AS policy_number,
    UPPER(asi.value) AS registration
FROM p_policy p
    JOIN p_pol_header ph ON ph.active_policy_id = p.id
        AND p.status_id = 20 -- 20=Policy (indicates active, not "Cancelled Policy", "Proposal" etc)
        AND ph.product_id = @productId
    JOIN p_policy_contact ppc ON p.id = ppc.policy_id    
        AND ppc.policy_contact_role = 6 -- 6=Policyholder
    JOIN cn_person cp ON ppc.contact_id = cp.CONTACT_ID
    JOIN cn_contact c ON cp.contact_id = c.id
    JOIN p_policy_lob ppl ON ph.active_policy_id = ppl.policy_id
    JOIN p_policy_lob_to_lob_asset ppltla ON ppl.ID = ppltla.policy_lob_id
    JOIN p_policy_lob_asset ppla ON ppltla.lob_asset_id = ppla.ID
    JOIN as_asset ass ON ppla.lob_asset_id = ass.ID
    JOIN as_asset_identifier asi ON asi.asset_id = ass.id AND asi.IDENTIFIER_TYPE_ID = 10000 -- Licence Plate
    JOIN CN_CONTACT_RACI cnr ON cnr.id = c.id
WHERE p.ENDORSMENT_TYPE_ID != 10 -- Exclude policies in renewal (10 = Policy Renewal)
    AND p.policy_start_date < DATEADD(month, -1, GETDATE()) -- Policy started more than 1 month ago
    AND asi.value IS NOT NULL
    AND asi.value != ''
    AND UPPER(asi.value) != 'TBA'
    AND c.first_name IS NOT NULL
    AND c.name IS NOT NULL
    AND cp.date_of_birth IS NOT NULL
    AND (cnr.WESTPAC_CUSTOMER_ID = c.id OR cnr.WESTPAC_CUSTOMER_ID IS NULL) -- Avoid contacts with bad tokens
ORDER BY NEWID();