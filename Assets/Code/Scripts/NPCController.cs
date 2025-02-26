using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] NPCUnitSO npc;
    [SerializeField] string currentDialogue;
    [SerializeField] float dialogueTime;
    [SerializeField] bool isAlreadyTalking;

    Transform protagonist;
    Coroutine conversationCoroutine;

    public TextMeshProUGUI dialogueText;

    List<string> conversation = new();

    private void Awake()
    {
        protagonist = null;
        dialogueText.enabled = false;

        for (int i = 0; i < npc.npcDialogues.Count; i++)
        {
            conversation.Add(npc.npcName + " : " + npc.npcDialogues[i]);
            conversation.Add("Kara : " + npc.playerResponses[i]);
        }
    }

    private void Update()
    {
        if (isAlreadyTalking && Vector3.Distance(transform.position, protagonist.position) > 1)
        {
            StopCoroutine(conversationCoroutine);
            currentDialogue = "";
            dialogueText.enabled = false;
            isAlreadyTalking = false;
        }
    }

    public void Interact(Transform protagonist)
    {
        if (!isAlreadyTalking)
        {
            this.protagonist = protagonist;
            dialogueText.enabled = true;
            conversationCoroutine = StartCoroutine(InitiateDialogue());
        }
    }

    public IEnumerator InitiateDialogue()
    {
        isAlreadyTalking = true;

        for (int i = 0; i < conversation.Count; i++)
        {
            currentDialogue = conversation[i];
            dialogueText.text = currentDialogue;
            dialogueTime = currentDialogue.Length / 20;
            yield return new WaitForSeconds(dialogueTime);
        }

        isAlreadyTalking = false;
        currentDialogue = "";
        dialogueText.enabled = false;
    }

}
