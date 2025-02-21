using UnityEngine;

public class PlayerInteractableController : MonoBehaviour
{

    [SerializeField] bool canInteract;
    public bool IsOperatingInteractable;
    public bool IsFixating;
    public bool CanClimb;

    IInteractable interactable = null;

    public GameObject interactionCanvas;
    public Transform fixatingTransform;

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

        if(CanClimb){
            
        }

        if (IsFixating && transform.position != fixatingTransform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, fixatingTransform.position, Time.deltaTime);
        }
        else if (IsFixating && transform.rotation != fixatingTransform.localRotation)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, fixatingTransform.rotation, 360 * Time.deltaTime);
        }
        else
        {
            IsFixating = false;
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
