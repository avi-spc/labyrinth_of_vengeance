using UnityEngine;
using UnityEngine.Events;

public class CombatantController : MonoBehaviour
{
    [SerializeField] CombatantUnitSO combatantUnit;
    [SerializeField] int healthPoints;

    public UnityEvent onDamageReceived;

    CombatantAI combatantAI;
    Animator animator;

    private void Awake()
    {
        healthPoints = combatantUnit.maxHealth;
        combatantAI = GetComponent<CombatantAI>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;

        onDamageReceived.Invoke();
    }

    public void ChangeState()
    {
        if (combatantAI.currentState != CombatantAI.State.Ranged)
        {
            combatantAI.currentState = CombatantAI.State.Ranged;
            combatantAI.suspicionLevel = 1.01f;
            combatantAI.isAlreadySuspecting = true;
            combatantAI.suspectedDistance = Vector3.Distance(transform.position, combatantAI.protagonist.position);
        }

        animator.SetTrigger("GotHit");
    }

    public void Shoot()
    {
            Debug.DrawRay(transform.position + transform.up, transform.forward * 10, Color.black);

            if (Physics.Raycast(transform.position + transform.up, transform.forward, out RaycastHit hitInfo, 10f, combatantAI.protagonistLayerMask))
            {
                hitInfo.transform.GetComponent<PlayerController>().health -= combatantUnit.damageValue;
            }
    }


}
