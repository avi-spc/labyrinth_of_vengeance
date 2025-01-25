using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CombatantAI : MonoBehaviour
{
    [SerializeField] float fieldOfView;
    [SerializeField] float viewDistance;
    [SerializeField] LayerMask obstacleLayerMask;

    [SerializeField] float moveSpeed;

    public Transform protagonist;
    CharacterController characterController;

    RaycastHit hitInfo;
    float patrolOffset = 5f;
    bool changingDirection;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        // DetectPlayer();
    }

    private void Update()
    {
        Patrol();
    }

    private void DetectPlayer()
    {
        Debug.DrawRay(transform.position, Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * viewDistance, Color.blue);
        Debug.DrawRay(transform.position, transform.forward * viewDistance, Color.blue);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * viewDistance, Color.blue);
        Debug.DrawRay(transform.position, protagonist.transform.position - transform.position, Color.yellow);
        Debug.DrawRay(transform.position, protagonist.transform.position - transform.position, Color.black);

        if (Vector3.Distance(transform.position, protagonist.transform.position) <= viewDistance && IsPlayerInViewRadius)
        {
            if (!Physics.Raycast(transform.position, protagonist.transform.position - transform.position, out hitInfo, viewDistance, obstacleLayerMask))
            {
                Debug.Log("Detected");
            }
            else
            {
                Debug.Log("Behind covers");
            }
        }
        else
        {
            Debug.Log("Out of sight");
        }
    }

    private void Patrol()
    {
        Debug.DrawRay(transform.position + transform.forward + transform.up, -transform.up * patrolOffset, Color.black);
        Debug.DrawRay(transform.position + transform.right + transform.up, transform.right * patrolOffset, Color.black);
        Debug.DrawRay(transform.position - transform.right + transform.up, -transform.right * patrolOffset, Color.black);

        if (!Physics.Raycast(transform.position + transform.forward + transform.up, -transform.up, patrolOffset, obstacleLayerMask))
        {
            if (!Physics.Raycast(transform.position + transform.right + transform.up, transform.right, patrolOffset, obstacleLayerMask) && !changingDirection)
            {
                changingDirection = true;
                transform.rotation *= Quaternion.Euler(0, 90, 0);
            }
            else if (!Physics.Raycast(transform.position - transform.right + transform.up, -transform.right, patrolOffset, obstacleLayerMask) && !changingDirection)
            {
                changingDirection = true;
                transform.rotation *= Quaternion.Euler(0, -90, 0);
            }

        }
        else
        {
            changingDirection = false;
            Move();
            Debug.Log(transform.forward);
        }
    }

    private void Move()
    {
        characterController.Move(transform.forward * moveSpeed * Time.deltaTime);
    }

    bool IsPlayerInViewRadius => Vector3.Angle(transform.forward, (protagonist.transform.position - transform.position).normalized) < fieldOfView / 2;
}
