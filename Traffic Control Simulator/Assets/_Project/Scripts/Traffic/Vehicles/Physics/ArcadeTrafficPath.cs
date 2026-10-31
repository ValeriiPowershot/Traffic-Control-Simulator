using UnityEngine;

// A bounded, allocation-free path over the already selected waypoint connection.
public sealed class ArcadeTrafficPath
{
    private readonly Vector3[] points = new Vector3[15];
    private int count;
    public float Curvature { get; private set; }
    public float Trim { get; private set; }
    public Vector3 Exit { get; private set; }
    public Vector3 ExitDirection { get; private set; }
    public static Vector3 Flat(Vector3 v) { v.y = 0; return v; }

    public void Build(Vector3 start, Vector3 corner, Vector3? next, float minimumRadius)
    {
        Vector3 incoming = Flat(corner - start);
        Curvature = Trim = 0;
        Exit = corner;
        ExitDirection = incoming.normalized;
        points[0] = start;
        if (!next.HasValue) { points[1] = corner; count = 2; return; }
        Vector3 outgoing = Flat(next.Value - corner);
        float angle = Vector3.Angle(incoming, outgoing) * Mathf.Deg2Rad;
        if (angle < 0.05f || angle > 2.8f || outgoing.sqrMagnitude < 0.01f || incoming.sqrMagnitude < 0.01f)
        { points[1] = corner; points[2] = next.Value; count = 3; return; }
        Trim = Mathf.Min(minimumRadius * Mathf.Tan(angle / 2) * 1.6f, Mathf.Min(incoming.magnitude, outgoing.magnitude) * 0.35f);
        Vector3 entry = corner - incoming.normalized * Trim;
        ExitDirection = outgoing.normalized;
        Exit = corner + ExitDirection * Trim;
        points[1] = entry;
        for (int i = 1; i <= 12; i++)
        {
            float t = i / 12f;
            points[i + 1] = (1 - t) * (1 - t) * entry + 2 * (1 - t) * t * corner + t * t * Exit;
        }
        points[14] = next.Value;
        count = 15;
        Curvature = Mathf.Sin(angle / 2) / Mathf.Max(0.1f, Trim * Mathf.Pow(Mathf.Cos(angle / 2), 2));
    }

    public Vector3 Target(Vector3 position, float lookAhead)
    {
        int nearestIndex = 0;
        float nearest = float.PositiveInfinity;
        Vector3 point = points[0];
        for (int i = 0; i < count - 1; i++)
        {
            Vector3 segment = Flat(points[i + 1] - points[i]);
            float t = segment.sqrMagnitude > 0.0001f ? Mathf.Clamp01(Vector3.Dot(Flat(position - points[i]), segment) / segment.sqrMagnitude) : 0;
            Vector3 candidate = points[i] + segment * t;
            float distance = Flat(candidate - position).sqrMagnitude;
            if (distance < nearest) { nearest = distance; nearestIndex = i; point = candidate; }
        }
        for (int i = nearestIndex; i < count - 1; i++)
        {
            Vector3 line = Flat(points[i + 1] - point);
            if (line.magnitude >= lookAhead) return point + line.normalized * lookAhead;
            lookAhead -= line.magnitude;
            point = points[i + 1];
        }
        return points[count - 1];
    }

    public void Draw()
    {
        for (int i = 0; i < count - 1; i++) Gizmos.DrawLine(points[i], points[i + 1]);
    }
}
