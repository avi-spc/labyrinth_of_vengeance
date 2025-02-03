using System;
using UnityEditor.Animations;
using UnityEngine;

public class ArtilleryController : MonoBehaviour
{
    [SerializeField] ArtilleryUnitSO equippedArtillery;
    [SerializeField] LayerMask combatantLayerMask;

    RaycastHit hitInfo;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            animator.SetBool("IsFiring", true);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            animator.SetBool("IsFiring", false);
        }
    }

    void Shoot()
    {
        Debug.DrawRay(transform.position + transform.up, transform.forward * equippedArtillery.fireRange, Color.red);

        if (Physics.Raycast(transform.position + transform.up, transform.forward, out hitInfo, equippedArtillery.fireRange, combatantLayerMask))
        {

            if (hitInfo.transform.TryGetComponent(out CombatantController combatant))
            {
                combatant.TakeDamage(equippedArtillery.damageValue);
            }
            // else if (hitInfo.transform.TryGetComponent(out DestructiblesController destructible))
            // {
            //     destructible.TakeDamage(equippedArtillery.damageValue);
            // }
        }

    }
}
