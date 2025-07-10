using Gley.TrafficSystem.Internal;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanAssets.Internal
{
    public abstract class GenericConnectionPool<T>: MonoBehaviour where T:ConnectionCurveBase
    {
        [SerializeField]
        private List<T> connectionCurves;


        public void AddConnection(T connectionCurve)
        {       
            connectionCurves.Add(connectionCurve);
        }


        public void RemoveConnection(T connectionCurve)
        {
            connectionCurves.Remove(connectionCurve);
        }


        public bool ContainsConnection(T connectionCurve)
        {
            return connectionCurves.Contains(connectionCurve);
        }


        public List<T> GetAllConnections()
        {
            return connectionCurves;
        }

        public void VerifyAssignments()
        {
            if (connectionCurves == null)
            {
                connectionCurves = new List<T>();
            }
        }
    }
}