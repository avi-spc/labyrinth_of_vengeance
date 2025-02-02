using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float moveForwardSpeed = 4f;
    [SerializeField] float moveBackwardSpeed = 1.5f;
    [SerializeField] float rotationSpeed = 360f;

    CharacterController characterController;
    Animator animator;

    public float health = 100f;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
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
    }

    private void LookTowards(float rotateInput)
    {
        Quaternion targetRotation = rotateInput < 0 ? Quaternion.Euler(0, -90, 0) : Quaternion.Euler(0, 90, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, transform.rotation * targetRotation, rotationSpeed * Time.deltaTime);
    }
}
