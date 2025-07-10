namespace Gley.UrbanAssets.Internal
{
    /// <summary>
    /// Stores lane properties
    /// </summary>
    [System.Serializable]
    public class Lane
    {
        public LaneConnectors laneEdges;
        public bool laneDirection;
        public int laneSpeed;
        public bool[] allowedCars;


        public Lane(int nrOfCars, bool laneDirection, int laneSpeed)
        {
            laneEdges = new LaneConnectors();
            this.laneDirection = laneDirection;
            this.laneSpeed = laneSpeed;
            allowedCars = new bool[nrOfCars];
            for (int i = 0; i < allowedCars.Length; i++)
            {
                allowedCars[i] = true;
            }
        }


        public void UpdateAllowedCars(int nrOfCars)
        {
            if (allowedCars.Length < nrOfCars)
            {
                bool[] newCars = new bool[nrOfCars];
                for (int i = 0; i < nrOfCars; i++)
                {
                    newCars[i] = true;
                }

                for (int i = 0; i < allowedCars.Length; i++)
                {
                    newCars[i] = allowedCars[i];
                }
                allowedCars = newCars;
            }
        }
    }


    [System.Serializable]
    public struct LaneConnectors
    {
        public WaypointSettingsBase inConnector;
        public WaypointSettingsBase outConnector;

        public LaneConnectors(WaypointSettingsBase  inConnector, WaypointSettingsBase outConnector)
        {
            this.inConnector = inConnector;
            this.outConnector = outConnector;
        }
    }
}