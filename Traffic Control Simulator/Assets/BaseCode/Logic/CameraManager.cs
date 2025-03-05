using DG.Tweening;
using UnityEngine;

namespace BaseCode.Logic
{
    public class CameraManager : ManagerBase
    {
        public Camera camera;
        
        public void CameraShake()
        {
            camera.transform.DOShakePosition(0.5f, 0.2f, 10, 90);
        }
        
    }
}