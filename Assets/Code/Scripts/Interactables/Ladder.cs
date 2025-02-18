using UnityEngine;
using UnityEngine.WSA;

public class Ladder : MonoBehaviour, IInteractable
{
    public Transform ladderBottom;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact(Transform protagonist)
    {
        Debug.Log("Hello");
        protagonist.position = ladderBottom.position;
        // protagonist.LookAt(ladderBottom.up, Vector3.up);
        protagonist.GetComponent<PlayerInteractableController>().IsOperatingInteractable = true;
    }
}
