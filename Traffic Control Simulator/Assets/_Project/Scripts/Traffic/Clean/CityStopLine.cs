using System;
using System.Collections.Generic;
using UnityEngine;

namespace TrafficControl.CleanVehicles
{
    public sealed class CityStopLine : MonoBehaviour
    {
        private static readonly List<CityStopLine> lines = new List<CityStopLine>();
        public static IReadOnlyList<CityStopLine> All => lines;
        public CityRoadPath Road;
        [Min(0)] public float Distance;
        public bool ManualClosed = true;
        private Func<bool> reader;
        public bool Closed => reader != null ? reader() : ManualClosed;
        public void Bind(Func<bool> stateReader) { reader = stateReader; }
        private void OnEnable() { if (!lines.Contains(this)) lines.Add(this); }
        private void OnDisable() { lines.Remove(this); }
    }
}
