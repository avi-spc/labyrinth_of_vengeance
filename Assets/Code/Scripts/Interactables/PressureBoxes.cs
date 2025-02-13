using UnityEngine;

public class PressureBoxes : MonoBehaviour, IInteractable
{
    [SerializeField] bool isHeld;

    public void Interact(Transform protagonist)
    {
        if (!isHeld)
        {
            transform.SetParent(protagonist);
            isHeld = true;
        }
        else
        {
            transform.parent = null;
            isHeld = false;
        }
    }

}
