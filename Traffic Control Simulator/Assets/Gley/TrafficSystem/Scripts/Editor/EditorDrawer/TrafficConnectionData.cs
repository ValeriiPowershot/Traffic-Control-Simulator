using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using System.Collections.Generic;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class TrafficConnectionData : Data
    {
        private TrafficRoadData roadData;
        private ConnectionPool[] allConnectionPools;
        private ConnectionCurve[] allConnections;
        private Dictionary<ConnectionCurve, WaypointSettings[]> connectionWaypoints;
        private Dictionary<ConnectionCurve, ConnectionPool> pools;


        internal TrafficConnectionData Initialize(TrafficRoadData roadData)
        {
            this.roadData = roadData;
            base.Initialize();
            return this;
        }


        internal ConnectionCurve[] GetAllConnections()
        {
            return allConnections;
        }


        internal ConnectionPool[] GetAllConnectionPools()
        {
            return allConnectionPools;
        }


        internal WaypointSettings[] GetWaypoints(ConnectionCurve connection)
        {
            connectionWaypoints.TryGetValue(connection, out var waypoints);
            return waypoints;
        }

        internal ConnectionPool GetConnectionPool(ConnectionCurve connection)
        {
            pools.TryGetValue(connection, out var pool);
            return pool;
        }


        protected override void LoadAllData()
        {
            var tempConnections = new List<ConnectionCurve>();
            var connectionPools = new List<ConnectionPool>();
            connectionWaypoints = new Dictionary<ConnectionCurve, WaypointSettings[]>();
            pools = new Dictionary<ConnectionCurve, ConnectionPool>();
            var allRoads = roadData.GetAllRoads();
            for (int i = 0; i < allRoads.Length; i++)
            {
                if (allRoads[i].isInsidePrefab && !GleyPrefabUtilities.EditingInsidePrefab())
                {
                    continue;
                }
                ConnectionPool connectionsScript = allRoads[i].transform.parent.GetComponent<ConnectionPool>();
                if(connectionsScript==null)
                {
                    Debug.Log(allRoads[i].name, allRoads[i].transform.parent);
                    continue;
                }
                if (!connectionPools.Contains(connectionsScript))
                {
                    connectionPools.Add(connectionsScript);
                }
            }

            //verify
            for (int i = 0; i < connectionPools.Count; i++)
            {
                connectionPools[i].VerifyAssignments();
                var connectionCurves = connectionPools[i].GetAllConnections();
                for (int j = connectionCurves.Count - 1; j >= 0; j--)
                {
                    if (connectionCurves[j].VerifyAssignments()==false)
                    {
                        if (connectionCurves[j].holder)
                        {
                            DestroyImmediate(connectionCurves[j].holder.gameObject);
                        }
                        connectionCurves.RemoveAt(j);
                    }
                    else
                    {
                        connectionCurves[j].inPosition = connectionCurves[j].GetInConnector().transform.position;
                        connectionCurves[j].outPosition = connectionCurves[j].GetOutConnector().transform.position;
                        
                        //add waypoints
                        var waypoints = new List<WaypointSettings>();
                        Transform waypointsHolder = connectionCurves[j].holder;
                        for (int k=0;k< waypointsHolder.childCount;k++)
                        {
                            var waypointScript = waypointsHolder.GetChild(k).GetComponent<WaypointSettings>();
                            if (waypointScript != null)
                            {
                                //check for null assignments assigned and remove them
                                waypointScript.VerifyAssignments();
                                waypointScript.position = waypointScript.transform.position;
                                waypoints.Add(waypointScript);
                            }
                        }
                        tempConnections.Add(connectionCurves[j]);
                        connectionWaypoints.Add(connectionCurves[j], waypoints.ToArray());
                        pools.Add(connectionCurves[j], connectionPools[i]);
                    }
                }
            }

            allConnectionPools = connectionPools.ToArray();
            allConnections = tempConnections.ToArray();
        }
    }
}
