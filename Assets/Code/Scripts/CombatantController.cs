using UnityEngine;
using UnityEngine.Events;

public class CombatantController : MonoBehaviour
{
    [SerializeField] CombatantUnitSO combatantUnit;
    [SerializeField] int healthPoints;

    public UnityEvent onDamageReceived;

    private void Awake()
    {
        healthPoints = combatantUnit.maxHealth;
    }

    public void TakeDamage(int damage)
    {
        healthPoints -= damage;

        onDamageReceived.Invoke();
    }
}
