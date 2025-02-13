using System.Collections;
using UnityEngine;

public class ElevatorSwitch : MonoBehaviour, IInteractable
{
    public ElevatorPlatform elevatorPlatform;

    public enum ElevatorOperatingState { Opened, Closed }
    public ElevatorOperatingState currentOperatingState;

    void Start()
    {
        currentOperatingState = ElevatorOperatingState.Closed;
    }

    public void Interact(Transform protagonist)
    {
        if (currentOperatingState == ElevatorOperatingState.Closed)
        {
            currentOperatingState = ElevatorOperatingState.Opened;
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
