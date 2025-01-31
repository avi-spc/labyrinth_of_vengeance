using UnityEngine;

[CreateAssetMenu(fileName = "ArtilleryUnitSO", menuName = "Scriptable Objects/Artillery Unit")]
public class ArtilleryUnitSO : ScriptableObject
{
    public float fireRate;
    public float fireRange;
    public float reloadTime;
    public int damageValue;
}
