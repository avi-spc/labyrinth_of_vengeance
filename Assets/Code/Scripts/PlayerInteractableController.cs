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
    public Transform destinationTransform;

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
        else if (Input.GetKeyDown(KeyCode.X) && CanClimb && Vector3.Distance(transform.position, fixatingTransform.position) < 0.025)
        {
            transform.GetComponent<Animator>().SetBool("IsClimbing", false);
            transform.position = Vector3.MoveTowards(transform.position, destinationTransform.position, 3 * Time.deltaTime);
            CanClimb = false;
            IsOperatingInteractable = false;
            interactable = null;
        }

        if (CanClimb && transform.position.y < destinationTransform.position.y)
        {
            float v = Input.GetAxisRaw("Vertical");
            float climbAmount = Mathf.Clamp(v, -1, 1);

            Vector3 moveInput = new Vector3(0, v, 0).normalized;

            transform.GetComponent<CharacterController>().Move(transform.rotation * moveInput * 1 * Time.deltaTime);
            transform.GetComponent<Animator>().SetFloat("climbAmount", climbAmount);

            Debug.Log(transform.position+" | " + fixatingTransform.position+" |");
        }
        else if (CanClimb && transform.position.y > destinationTransform.position.y)
        {
            transform.position = Vector3.MoveTowards(transform.position, destinationTransform.position, 2 * Time.deltaTime);
            transform.GetComponent<Animator>().SetTrigger("ReachedTop");

            Debug.Log(transform.position + "" + destinationTransform.position);
        }
        else if (CanClimb && transform.position == destinationTransform.position)
        {
            transform.GetComponent<Animator>().ResetTrigger("ReachedTop");

            IsOperatingInteractable = false;
            CanClimb = false;
            transform.GetComponent<Animator>().SetBool("IsClimbing", false);
        }

        if (IsFixating && transform.position != fixatingTransform.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, fixatingTransform.position, 3 * Time.deltaTime);
        }
        else if (IsFixating && transform.rotation != fixatingTransform.rotation)
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
