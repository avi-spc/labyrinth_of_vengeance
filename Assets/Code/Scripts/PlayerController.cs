using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveForwardSpeed = 3f;
    [SerializeField] float moveBackwardSpeed = 1.5f;
    [SerializeField] float rotationSpeed = 360f;

    CharacterController characterController;
    Animator animator;

    public float health = 100f;

    public bool isStealthAvailable;
    public Transform stealthTarget;
    Transform detectorPosition;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        detectorPosition = transform.GetChild(2);
    }

    void Update()
    {
        if (!transform.GetComponent<PlayerInteractableController>().IsOperatingInteractable)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            float moveAmount = Mathf.Clamp(v, -1, 1);

            Vector3 moveInput = new Vector3(0, 0, v).normalized;

            if (h != 0)
            {
                LookTowards(h);
            }

            float moveSpeed = v < 0 ? moveBackwardSpeed : moveForwardSpeed;

            characterController.Move(transform.rotation * moveInput * moveSpeed * Time.deltaTime);
            animator.SetFloat("moveAmount", moveAmount, 0.1f, Time.deltaTime);

            if (Input.GetKey(KeyCode.F) && stealthTarget.GetComponent<CombatantAI>().currentState == CombatantAI.State.Patrol && animator.GetBool("IsCrouched"))
            {
                StealthAttack();
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                animator.SetBool("IsCrouched", true);
                moveForwardSpeed = 1f;
                moveBackwardSpeed = .5f;
                characterController.height = 0.85f;
                characterController.center = new Vector3(0, 0.4f, 0);
                detectorPosition.localPosition += Vector3.down;
            }

            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                animator.SetBool("IsCrouched", false);
                moveForwardSpeed = 3f;
                moveBackwardSpeed = 1f;
                characterController.height = 1.7f;
                characterController.center = new Vector3(0, 0.85f, 0);
                detectorPosition.localPosition += Vector3.up;

            }
        }
    }

    private void LookTowards(float rotateInput)
    {
        Quaternion targetRotation = rotateInput < 0 ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, 90, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.rotation * targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void StealthAttack()
    {
        animator.SetTrigger("StealthKill");
        stealthTarget.transform.position = transform.position + transform.forward * 1.5f;
        stealthTarget.transform.LookAt(2 * stealthTarget.transform.position - transform.position, Vector3.up);
        stealthTarget.GetComponent<CombatantController>().HandleStealthDeath();
        isStealthAvailable = false;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Combatant"))
        {
            stealthTarget = collider.transform;
            // isStealthAvailable = stealthTarget.GetComponent<CombatantAI>().currentState == CombatantAI.State.Patrol;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Combatant"))
        {
            isStealthAvailable = false;
            stealthTarget = null;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        animator.SetTrigger("GotHit");
    }
}
