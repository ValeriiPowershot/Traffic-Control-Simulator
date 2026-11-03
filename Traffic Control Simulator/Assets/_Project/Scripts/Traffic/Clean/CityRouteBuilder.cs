using System.Collections.Generic;
using UnityEngine;

namespace TrafficControl.CleanVehicles
{
    // Converts survey points to straight segments and tangent circular corner arcs.
    // Used once when authoring, never depends on a legacy vehicle at runtime.
    public static class CityRouteBuilder
    {
        public static CityRoadPath.Curve[] Build(IList<Vector3> points, float cornerTrim = 5, float speedKph = 30)
        {
            var curves = new List<CityRoadPath.Curve>();
            var clean = new List<Vector3>();
            foreach (var point in points)
                if (clean.Count == 0 || Vector3.Distance(point, clean[clean.Count - 1]) > 0.1f) clean.Add(point);
            if (clean.Count < 2) return curves.ToArray();
            Vector3 cursor = clean[0];
            for (int i = 1; i < clean.Count - 1; i++)
            {
                Vector3 incoming = clean[i] - clean[i - 1], outgoing = clean[i + 1] - clean[i];
                float angle = Vector3.Angle(incoming, outgoing) * Mathf.Deg2Rad;
                if (angle < 0.03f || angle > 2.8f)
                { curves.Add(CityRoadPath.Curve.Straight(cursor, clean[i], speedKph)); cursor = clean[i]; continue; }
                float trim = Mathf.Min(cornerTrim, Mathf.Min(incoming.magnitude, outgoing.magnitude) * 0.4f);
                float radius = trim / Mathf.Tan(angle * 0.5f);
                float handle = 4f / 3f * Mathf.Tan(angle * 0.25f) * radius;
                Vector3 entry = clean[i] - incoming.normalized * trim, exit = clean[i] + outgoing.normalized * trim;
                if (Vector3.Distance(cursor, entry) > 0.01f) curves.Add(CityRoadPath.Curve.Straight(cursor, entry, speedKph));
                curves.Add(new CityRoadPath.Curve { Start = entry, HandleA = entry + incoming.normalized * handle,
                    HandleB = exit - outgoing.normalized * handle, End = exit, SpeedKph = speedKph });
                cursor = exit;
            }
            curves.Add(CityRoadPath.Curve.Straight(cursor, clean[clean.Count - 1], speedKph));
            return curves.ToArray();
        }
    }
}
