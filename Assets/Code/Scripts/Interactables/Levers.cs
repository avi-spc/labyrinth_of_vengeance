using UnityEngine;

public class Levers : MonoBehaviour, IInteractable
{
    [SerializeField] bool isSwitchedOn;

    public void Interact(Transform protagonist)
    {
        if (isSwitchedOn)
        {
            isSwitchedOn = false;
        }
        else
        {
            isSwitchedOn = true;
        }

        Debug.Log(isSwitchedOn);
    }
}
