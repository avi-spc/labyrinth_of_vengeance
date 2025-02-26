using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NPCUnitSO", menuName = "Scriptable Objects/NPC Unit")]
public class NPCUnitSO : ScriptableObject
{
    public string npcName;
    public List<string> npcDialogues;
    public List<string> playerResponses;
}
