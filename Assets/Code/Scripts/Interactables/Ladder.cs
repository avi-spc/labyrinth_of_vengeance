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
        protagonist.GetComponent<PlayerInteractableController>().IsOperatingInteractable = true;
        Debug.Log(transform.TransformPoint(ladderBottom.position));
        Debug.Log(ladderBottom.position);
        protagonist.GetComponent<PlayerInteractableController>().fixatingTransform = ladderBottom;
        protagonist.GetComponent<PlayerInteractableController>().IsFixating = true;
        // protagonist.position = ladderBottom.position;
    }
}
