using System;
using UnityEditor.Animations;
using UnityEngine;

public class ArtilleryController : MonoBehaviour
{
    [SerializeField] ArtilleryUnitSO equippedArtillery;
    [SerializeField] LayerMask combatantLayerMask;

    RaycastHit hitInfo;
    float previousFireTimestamp;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButton("Fire1") && CanShoot)
        {
            Shoot();
        }

        if (Input.GetButtonUp("Fire1"))
        {
            animator.SetLayerWeight(1, 0);
        }
    }

    void Shoot()
    {
        previousFireTimestamp = Time.time;

        Debug.DrawRay(transform.position + transform.up, transform.forward * equippedArtillery.fireRange, Color.red);
        animator.SetLayerWeight(1, 1);

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

    bool CanShoot => Time.time - previousFireTimestamp > equippedArtillery.fireRate;
}
