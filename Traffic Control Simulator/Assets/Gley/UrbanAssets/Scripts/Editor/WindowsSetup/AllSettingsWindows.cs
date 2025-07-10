using System.Linq;

namespace Gley.UrbanAssets.Editor
{
    public class AllSettingsWindows
    {
        WindowProperties[] allWindows;


        internal void Initialize(WindowProperties[] allWindowsProperties)
        {
            allWindows = allWindowsProperties; 
        }


        internal WindowProperties GetWindowProperties(string className)
        {
            return allWindows.First(cond => cond.className == className);
        }


        internal string GetWindowName(string className)
        {
            return allWindows.First(cond => cond.className == className).title;
        }
    }
}
