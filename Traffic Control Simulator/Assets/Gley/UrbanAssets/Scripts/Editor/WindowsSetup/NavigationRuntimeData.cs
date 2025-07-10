using System.Collections.Generic;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public class NavigationRuntimeData
    {
        private List<string> path;
        private AllSettingsWindows allSettingsWindows;


        public NavigationRuntimeData(AllSettingsWindows allSettingsWindows)
        {
            this.allSettingsWindows = allSettingsWindows;
            path = new List<string>();
        }


        internal void AddWindow(string newWindow)
        {
            if (!path.Contains(newWindow))
            {
                path.Add(newWindow);
            }
            else
            {
                Debug.LogWarning("Trying to add same window twice: " + newWindow);
            }
        }


        internal string GetBackPath()
        {
            if (path.Count == 0)
                return "";

            string result = "";
            for (int i = 0; i < path.Count; i++)
            {
                result += allSettingsWindows.GetWindowName(path[i].Split('.')[3]) + " > ";
            }
            return result;
        }


        internal string RemoveLastWindow()
        {
            string lastWindow = path[path.Count - 1];

            path.RemoveAt(path.Count - 1);
            return lastWindow;
        }
    }
}
