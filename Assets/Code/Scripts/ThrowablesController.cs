using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowablesController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vector3> lineRendererSegments = new();

    public GameObject equippedThrowable;

    private void Start()
    {
        lineRenderer.positionCount = BezierTrajectory.numPoints;
    }

    void Update()
    {
        if (Mathf.Abs(Input.GetAxis("Mouse X")) > 0.01f || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.01f)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Debug.Log(hit.point);
                lineRendererSegments = BezierTrajectory.CalculateProjectileTrajectory(transform.position, hit.point);
                lineRenderer.SetPositions(lineRendererSegments.ToArray());
            }
        }

        if (Input.GetButtonDown("Fire1"))
        {
            StartCoroutine(ReleaseThrowable(lineRendererSegments));
        }
    }

    private IEnumerator ReleaseThrowable(List<Vector3> trajectory)
    {
        float timeInterval = Vector3.Distance(trajectory[0], trajectory[trajectory.Count - 1]) / 800;

        foreach (Vector3 point in trajectory)
        {
            equippedThrowable.transform.position = point;
            yield return new WaitForSeconds(timeInterval);
        }
    }
}
