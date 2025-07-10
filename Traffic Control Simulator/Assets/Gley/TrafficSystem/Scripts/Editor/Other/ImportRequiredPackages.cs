using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Events;

namespace Gley.TrafficSystem.Editor
{
    public class ImportRequiredPackages
    {
        private static AddRequest Request;
        private static UnityAction<string> UpdateMethod;


        public static void ImportPackages(UnityAction<string> UpdateMethod)
        {
            ImportRequiredPackages.UpdateMethod = UpdateMethod;
            Debug.Log("Installation started. Please wait");
            Request = UnityEditor.PackageManager.Client.Add("com.unity.burst");
            EditorApplication.update += Progress;
        }


        private static void Progress()
        {
            UpdateMethod(Request.Status.ToString());
            if (Request.IsCompleted)
            {
                if (Request.Status == UnityEditor.PackageManager.StatusCode.Success)
                {
                    Debug.Log("Installed: " + Request.Result.packageId);
                    UpdateMethod("Installed: " + Request.Result.packageId);
                }
                else
                {
                    if (Request.Status >= UnityEditor.PackageManager.StatusCode.Failure)
                    {
                        Debug.Log(Request.Error.message);
                        UpdateMethod(Request.Error.message);

                    }
                }
                EditorApplication.update -= Progress;
            }
        }
    }
}
