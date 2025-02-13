using UnityEngine;

public class Crane : MonoBehaviour
{
    public CraneController craneController;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Obstacles"))
        {
            craneController.currentOperatingState = CraneController.OperatingState.Picking;
            collider.transform.SetParent(transform);
        }
    }
}
