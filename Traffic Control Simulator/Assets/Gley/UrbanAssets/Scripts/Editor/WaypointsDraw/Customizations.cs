using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public static class Customizations 
    {
        const float referenceDistance = 35;
        const float anchorSize = 0.5f;
        const float controlSize = 1;
        const float roadConnectorSize = 1;


        public static float GetZoomPercentage(Vector3 cameraPoz, Vector3 objPoz)
        {
            float cameraDistace = Vector3.Distance(cameraPoz, objPoz);
            return  cameraDistace / referenceDistance;
        }


        public static float GetRoadConnectorSize(Vector3 camPoz, Vector3 objPoz)
        {
            return GetZoomPercentage(camPoz,objPoz) * roadConnectorSize;
        }


        public static float GetControlPointSize(Vector3 camPoz, Vector3 objPoz)
        {
            return GetZoomPercentage(camPoz,objPoz) * controlSize;
        }


        public static float GetAnchorPointSize(Vector3 camPoz, Vector3 objPoz)
        {
            return GetZoomPercentage(camPoz, objPoz) * anchorSize;
        }
    }
}
