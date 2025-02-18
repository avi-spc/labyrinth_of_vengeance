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
            transform.position += new Vector3(0, 1f, 0) * Time.deltaTime;
        else if (transform.position.y >= destinationFloor.position.y)
        {
            destinationFloor.GetComponent<Animator>().SetTrigger("OpenOnce");
        }
        else
        {
            isPowered = false;
            //Open the Elevator Door
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Elevator"))
        {
            collider.transform.parent.SetParent(transform);
            isInside = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Elevator"))
        {
            collider.transform.parent.SetParent(null);
            isInside = false;
        }
    }
}
