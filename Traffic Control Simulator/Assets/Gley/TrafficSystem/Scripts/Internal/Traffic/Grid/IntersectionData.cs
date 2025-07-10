namespace Gley.TrafficSystem.Internal
{
    /// <summary>
    /// Structure used to store intersection to grid
    /// </summary>
    [System.Serializable]
    public struct IntersectionData
    {
        public IntersectionType type;
        public int index;

        public IntersectionData(IntersectionType type, int index)
        {
            this.type = type;
            this.index = index;
        }
    }
}
