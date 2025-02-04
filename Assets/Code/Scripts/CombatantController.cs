using UnityEngine;
using UnityEngine.Events;

public class CombatantController : MonoBehaviour
{
    [SerializeField] CombatantUnitSO combatantUnit;
    [SerializeField] int health;

    public UnityEvent onDamageReceived;

    CombatantAI combatantAI;
    Animator animator;

    private void Awake()
    {
        health = combatantUnit.maxHealth;
        combatantAI = GetComponent<CombatantAI>();
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

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
        HandleDeath();
    }

    public void Shoot()
    {
        Debug.DrawRay(transform.position + transform.up, transform.forward * 10, Color.black);

        if (Physics.Raycast(transform.position + transform.up, transform.forward, out RaycastHit hitInfo, 10f, combatantAI.protagonistLayerMask))
        {
            hitInfo.transform.GetComponent<PlayerController>().health -= combatantUnit.damageValue;
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Grenade"))
        {
            health -= 100;
        }

        HandleDeath();
    }

    private void HandleDeath()
    {
        if (health <= 0)
        {
            GetComponent<CombatantAI>().enabled = false;
            GetComponent<Collider>().enabled = false;
            animator.SetTrigger("Die");
        }
    }
}
