using System.Collections;
using UnityEngine;

public class ElevatorSwitch : MonoBehaviour, IInteractable
{
    public ElevatorPlatform elevatorPlatform;

    public enum ElevatorOperatingState { Closed, Opened }
    public ElevatorOperatingState currentOperatingState;

    Animator animator;

    void Start()
    {
        currentOperatingState = ElevatorOperatingState.Closed;
        animator = transform.parent.GetComponent<Animator>();
    }

    public void Interact(Transform protagonist)
    {
        if (currentOperatingState == ElevatorOperatingState.Closed)
        {
            currentOperatingState = ElevatorOperatingState.Opened;
            animator.SetTrigger("Open");
            StartCoroutine(OpenCloseElevator());
        }
    }

    private IEnumerator OpenCloseElevator()
    {
        yield return new WaitForSeconds(4);
        elevatorPlatform.isPowered = true;
        currentOperatingState = ElevatorOperatingState.Closed;
    }

}
