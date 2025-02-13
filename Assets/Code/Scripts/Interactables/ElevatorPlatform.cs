using System.Collections;
using UnityEngine;

public class ElevatorPlatform : MonoBehaviour
{
    public Transform destinationFloor;

    public bool isInside;
    public bool isPowered;

    void FixedUpdate()
    {
        if (transform.position.y < destinationFloor.position.y && isInside && isPowered)
            transform.position += new Vector3(0, 0.5f, 0) * Time.deltaTime;
        else
        {
            isPowered = false;
            //Open the Elevator Door
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.transform.SetParent(transform);
            isInside = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            collider.transform.SetParent(null);
            isInside = false;
        }
    }
}
