using UnityEngine;

[CreateAssetMenu(fileName = "CombatantUnitSO", menuName = "Scriptable Objects/Combatant Unit")]
public class CombatantUnitSO : ScriptableObject
{
    public string combatantType;
    
    public int maxHealth;
    public int damageValue;

}
