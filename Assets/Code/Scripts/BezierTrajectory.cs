using System.Collections.Generic;
using UnityEngine;

public static class BezierTrajectory
{
    public static readonly int numPoints = 50;

    public static List<Vector3> CalculateProjectileTrajectory(Vector3 source, Vector3 handle, Vector3 target)
    {
        List<Vector3> trajectorySegments = new()
        {
            source
        };

        for (int i = 1; i <= numPoints; i++)
        {
            float t = i / (float)numPoints;

            Vector3 newPosition = Mathf.Pow(1 - t, 2) * source + 2 * Mathf.Pow(1 - t, 1) * t * handle + Mathf.Pow(t, 2) * target;
            // Debug.DrawRay(positionSegments[positionSegments.Count - 1], newPosition - positionSegments[positionSegments.Count - 1], Color.black);
            trajectorySegments.Add(newPosition);
        }

        return trajectorySegments;
    }
}