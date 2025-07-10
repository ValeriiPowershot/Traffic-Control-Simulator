namespace Gley.UrbanAssets.Internal
{
    /// <summary>
    /// A row of cells
    /// </summary>
    [System.Serializable]
    public class GridRow 
    {
        public GridCell[] row;
        public GridRow(int nrOfElements)
        {
            row = new GridCell[nrOfElements];
        }
    }
}