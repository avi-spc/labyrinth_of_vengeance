using System.Collections.Generic;
using UnityEngine;

public class ThrowablesController : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public List<Vector3> lineRendererSegments = new();

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

    }
}
