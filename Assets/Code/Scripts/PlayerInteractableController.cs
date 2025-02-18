using UnityEngine;

public class PlayerInteractableController : MonoBehaviour
{

    [SerializeField] bool canInteract;
    public bool IsOperatingInteractable;

    IInteractable interactable = null;

    public GameObject interactionCanvas;

    void Start()
    {
        interactionCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && canInteract && interactable != null)
        {
            interactable.Interact(transform);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Interactable"))
        {
            if (collider.TryGetComponent<IInteractable>(out IInteractable component))
            {
                canInteract = true;
                interactable = component;
                interactionCanvas.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Interactable"))
        {
            if (collider.TryGetComponent<IInteractable>(out IInteractable component))
            {
                canInteract = false;
                interactable = null;
                interactionCanvas.SetActive(false);
            }
        }
    }
}
