using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficControl.CleanVehicles
{
    public sealed class CityRoadPath : MonoBehaviour
    {
        [Serializable]
        public struct Curve
        {
            public Vector3 Start, HandleA, HandleB, End;
            [Min(1)] public float SpeedKph;
            public static Curve Straight(Vector3 a, Vector3 b, float speed = 30)
                => new Curve { Start = a, HandleA = Vector3.Lerp(a, b, 1f / 3), HandleB = Vector3.Lerp(a, b, 2f / 3), End = b, SpeedKph = speed };
        }
        public struct Sample
        {
            public Vector3 Position, Tangent;
            public float Distance, Curvature, Speed;
        }
        public Curve[] Curves = new Curve[0];
        [Range(8, 100)] public int Resolution = 40;
        public Color GuideColor = Color.cyan;
        private Sample[] samples;
        public float Length { get { Ensure(); return samples.Length > 0 ? samples[samples.Length - 1].Distance : 0; } }
        private void Ensure() { if (samples == null) Rebuild(); }
        public void Rebuild()
        {
            var result = new List<Sample>();
            float distance = 0;
            if (Curves != null) foreach (var curve in Curves)
            {
                Vector3 p0 = transform.TransformPoint(curve.Start), p1 = transform.TransformPoint(curve.HandleA);
                Vector3 p2 = transform.TransformPoint(curve.HandleB), p3 = transform.TransformPoint(curve.End);
                for (int i = 0; i <= Mathf.Max(8, Resolution); i++)
                {
                    if (result.Count > 0 && i == 0) continue;
                    float t = i / (float)Mathf.Max(8, Resolution), u = 1 - t;
                    Vector3 point = u * u * u * p0 + 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t * p3;
                    Vector3 first = 3 * u * u * (p1 - p0) + 6 * u * t * (p2 - p1) + 3 * t * t * (p3 - p2);
                    Vector3 second = 6 * u * (p2 - 2 * p1 + p0) + 6 * t * (p3 - 2 * p2 + p1);
                    first.y = second.y = 0;
                    if (result.Count > 0) distance += Vector3.Distance(result[result.Count - 1].Position, point);
                    result.Add(new Sample { Position = point, Tangent = first.sqrMagnitude > 0.0001f ? first.normalized : Vector3.forward,
                        Distance = distance, Curvature = Vector3.Cross(first, second).y / Mathf.Max(0.0001f, Mathf.Pow(first.magnitude, 3)), Speed = Mathf.Max(1, curve.SpeedKph) / 3.6f });
                }
            }
            samples = result.ToArray();
        }
        public Sample At(float distance)
        {
            Ensure();
            if (samples.Length == 0) return new Sample { Position = transform.position, Tangent = transform.forward };
            if (distance <= 0) return samples[0];
            if (distance >= Length) return samples[samples.Length - 1];
            int low = 0, high = samples.Length - 1;
            while (high - low > 1) { int mid = (low + high) / 2; if (samples[mid].Distance < distance) low = mid; else high = mid; }
            Sample a = samples[low], b = samples[high];
            float t = Mathf.InverseLerp(a.Distance, b.Distance, distance);
            return new Sample { Position = Vector3.Lerp(a.Position, b.Position, t), Tangent = Vector3.Lerp(a.Tangent, b.Tangent, t).normalized,
                Distance = distance, Curvature = Mathf.Lerp(a.Curvature, b.Curvature, t), Speed = Mathf.Min(a.Speed, b.Speed) };
        }
        public float Project(Vector3 position, float hint, bool fullSearch = false)
        {
            Ensure();
            float best = float.PositiveInfinity, distance = Mathf.Clamp(hint, 0, Length);
            for (int i = 0; i < samples.Length - 1; i++)
            {
                Sample a = samples[i], b = samples[i + 1];
                if (!fullSearch && (b.Distance < hint - 3 || a.Distance > hint + 12)) continue;
                Vector3 line = b.Position - a.Position; line.y = 0;
                Vector3 offset = position - a.Position; offset.y = 0;
                float t = line.sqrMagnitude > 0.0001f ? Mathf.Clamp01(Vector3.Dot(offset, line) / line.sqrMagnitude) : 0;
                float error = (offset - line * t).sqrMagnitude;
                if (error < best) { best = error; distance = Mathf.Lerp(a.Distance, b.Distance, t); }
            }
            return distance;
        }
        private void OnValidate() { samples = null; }
        private void OnDrawGizmosSelected()
        {
            Rebuild(); Gizmos.color = GuideColor;
            for (int i = 1; i < samples.Length; i++) Gizmos.DrawLine(samples[i - 1].Position, samples[i].Position);
        }
    }
}
