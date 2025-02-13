using System.Collections;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class CraneController : MonoBehaviour, IInteractable
{
    public GameObject controlledCrane;
    public float initialHeight;

    [SerializeField] bool isBeingOperated;
    [SerializeField] bool hasPicked;

    public enum OperatingState { Idle, Transit, Descending, Ascending, Picking, Holding };
    public OperatingState currentOperatingState;

    void Start()
    {
        currentOperatingState = OperatingState.Idle;
        controlledCrane.GetComponent<Crane>().craneController = this;
        initialHeight = controlledCrane.transform.position.y;
    }

    void Update()
    {
        if (isBeingOperated)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Vector3 moveInput = new Vector3(h, 0, v).normalized;
            if (Mathf.Abs(h) > Mathf.Abs(v))
            {
                moveInput.z = 0;
            }
            else
            {
                moveInput.x = 0;
            }

            controlledCrane.transform.position += moveInput * Time.deltaTime * 3;

            if (Input.GetKeyDown(KeyCode.Mouse1) && currentOperatingState == OperatingState.Transit)
            {
                StartCoroutine(Descend());
            }

            if (Input.GetKeyDown(KeyCode.Mouse1) && currentOperatingState == OperatingState.Holding)
            {
                Debug.Log("Drop");
            }
        }
    }

    public void Interact(Transform protagonist)
    {
        if (!isBeingOperated)
        {
            protagonist.GetComponent<PlayerInteractableController>().IsOperatingInteractable = true;
            isBeingOperated = true;
            currentOperatingState = OperatingState.Transit;
        }
        else
        {
            protagonist.GetComponent<PlayerInteractableController>().IsOperatingInteractable = false;
            isBeingOperated = false;
            currentOperatingState = OperatingState.Idle;
        }
    }

    private IEnumerator Ascend()
    {
        while (controlledCrane.transform.position.y != initialHeight)
        {
            controlledCrane.transform.position += new Vector3(0, .05f, 0);
            yield return new WaitForSeconds(0.01f);
        }

        currentOperatingState = OperatingState.Holding;
    }

    private IEnumerator Descend()
    {
        while (currentOperatingState != OperatingState.Picking)
        {
            controlledCrane.transform.position -= new Vector3(0, .05f, 0);
            yield return new WaitForSeconds(0.01f);
        }

        StartCoroutine(Ascend());
    }
}
