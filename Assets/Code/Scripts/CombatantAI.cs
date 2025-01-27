using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class CombatantAI : MonoBehaviour
{
    [SerializeField] float fieldOfView;
    [SerializeField] float viewDistance;
    [SerializeField] LayerMask obstacleLayerMask;
    [SerializeField] LayerMask platformLayerMask;

    [SerializeField] float moveSpeed;

    public Transform protagonist;
    CharacterController characterController;

    RaycastHit hitInfo;
    float patrolOffset = 1f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void FixedUpdate()
    {
        // DetectPlayer();
        Patrol();
    }

    private void Update()
    {
    }

    private void Patrol()
    {
        Debug.DrawRay(transform.position + transform.up, -transform.right * patrolOffset, Color.black);
        Debug.DrawRay(transform.position + transform.up, Quaternion.Euler(0, 45, 0) * -transform.right * patrolOffset * 2, Color.black);
        Debug.DrawRay(transform.position + transform.up - transform.forward / 2, -transform.right * patrolOffset, Color.black);
        Debug.DrawRay(transform.position + transform.up - transform.right, -transform.up * patrolOffset * 4, Color.black);


        Debug.DrawRay(transform.position + transform.forward / 2 + transform.up, -transform.up * patrolOffset * 4, Color.black);
        Debug.DrawRay(transform.position + transform.up, transform.forward * patrolOffset, Color.black);

        bool obstacleFront = Physics.Raycast(transform.position + transform.up, transform.forward, patrolOffset, obstacleLayerMask);
        bool hasGround = Physics.Raycast(transform.position + transform.forward / 2 + transform.up, -transform.up, patrolOffset * 4, platformLayerMask);
        bool noLeftObstacle = !Physics.Raycast(transform.position + transform.up, -transform.right, patrolOffset, obstacleLayerMask) && !Physics.Raycast(transform.position + transform.up, Quaternion.Euler(0, 45, 0) * -transform.right, patrolOffset * 2, obstacleLayerMask) && !Physics.Raycast(transform.position + transform.up - transform.forward / 2, -transform.right, patrolOffset, obstacleLayerMask) && Physics.Raycast(transform.position - transform.right + transform.up, -transform.up, patrolOffset * 4, platformLayerMask);

        if (noLeftObstacle)
        {
            transform.rotation *= Quaternion.Euler(0, -90, 0);
        }
        else if (hasGround && !obstacleFront)
        {
            Move();
        }
        else
        {
            transform.rotation *= Quaternion.Euler(0, 90, 0);
        }
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

    private void Move()
    {
        characterController.Move(transform.forward * moveSpeed * Time.deltaTime);
    }

    bool IsPlayerInViewRadius => Vector3.Angle(transform.forward, (protagonist.transform.position - transform.position).normalized) < fieldOfView / 2;
}
