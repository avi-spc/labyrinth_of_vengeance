using UnityEngine;
using UnityEngine.WSA;

public class Ladder : MonoBehaviour, IInteractable
{
    public Transform climberBottom, climberTop;

    public void Interact(Transform protagonist)
    {
        protagonist.GetComponent<PlayerInteractableController>().IsOperatingInteractable = true;
        Debug.Log(transform.TransformPoint(climberBottom.position));
        Debug.Log(climberBottom.position);
        protagonist.GetComponent<PlayerInteractableController>().fixatingTransform = climberBottom;
        protagonist.GetComponent<PlayerInteractableController>().destinationTransform = climberTop;
        protagonist.GetComponent<PlayerInteractableController>().IsFixating = true;
        protagonist.GetComponent<PlayerInteractableController>().CanClimb = true;
        protagonist.GetComponent<Animator>().SetBool("IsClimbing", true);

        // protagonist.position = ladderBottom.position;
    }
}
