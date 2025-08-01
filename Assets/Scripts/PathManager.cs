using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOnPath
{
    public int index; // index of the start point of current edge
    public float t; // t value of the point on the edge, 0 <= t <= 1
    public bool reachedEnd; // whether the point has reached the end of the path
    public Vector2 GetPosition()
    {
        return PathManager.Instance.GetPosition(this);
    }

    public void MoveBy(float distance)
    {
        PathManager.Instance.MovePPByAmount(this, distance);
    }
}

public class PathManager : SingletonClass<PathManager>
{
    public List<Vector2> points = new List<Vector2>();


    public Vector2 GetPosition(PointOnPath pp)
    {
        float distance = pp.t * Vector2.Distance(points[pp.index], points[pp.index + 1]);
        Vector2 direction = (points[pp.index + 1] - points[pp.index]).normalized;
        return points[pp.index] + direction * distance;
    }

    public void MovePPByAmount(PointOnPath pp, float distance)
    {
        if(pp.reachedEnd) return; // If already reached the end, do nothing
        float edgeLength = Vector2.Distance(points[pp.index], points[pp.index + 1]);
        pp.t += distance / edgeLength;
        if (pp.t >= 1)
        {
            float extraDistance = (pp.t - 1) * edgeLength;
            pp.index++;
            if (pp.index >= points.Count - 1)
            {
                pp.reachedEnd = true;
                pp.index = points.Count - 2; // Clamp to the last edge
                pp.t = 1; // Set t to 1 at the end of the path
                return;
            }
            edgeLength = Vector2.Distance(points[pp.index], points[pp.index + 1]);
            pp.t = extraDistance / edgeLength;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < points.Count - 1; i++)
        {
            Gizmos.DrawLine(points[i], points[i + 1]);
        }
        for (int i = 0; i < points.Count; i++)
        {
            Gizmos.DrawSphere(points[i], 0.1f);
        }
    }
}
