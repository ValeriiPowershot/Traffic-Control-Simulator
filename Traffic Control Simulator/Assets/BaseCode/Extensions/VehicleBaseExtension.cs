using System.Collections;
using System.Collections.Generic;
using BaseCode.Logic.Vehicles.Vehicles;
using UnityEngine;

namespace BaseCode.Extensions
{
    public static class VehicleBaseExtension
    {
        public static VehicleBase DisableVehicle(this VehicleBase vehicle)
        {
            vehicle.enabled = false;
            return vehicle;
        }
        public static VehicleBase EnableVehicle(this VehicleBase vehicle)
        {
            vehicle.enabled = true;
            return vehicle;
        }
        public static VehicleBase DisableVehicleWithCollider(this VehicleBase vehicle)
        {
            vehicle.enabled = false;
            DisableVehicleCollider(vehicle);
            return vehicle;
        }
        public static VehicleBase EnableVehicleWithCollider(this VehicleBase vehicle)
        {
            vehicle.enabled = true;
            EnableVehicleCollider(vehicle);
            return vehicle;
        }
        
        
        public static VehicleBase EnableVehicleCollider(this VehicleBase vehicle)
        {
            vehicle.GetComponent<BoxCollider>().enabled = true;
            return vehicle;
        }
        public static VehicleBase DisableVehicleCollider(this VehicleBase vehicle)
        {
            vehicle.GetComponent<BoxCollider>().enabled = false;
            return vehicle;
        }
        
        public static IEnumerator FadeOutAndRemove(this VehicleBase vehicle)
        {
            Renderer[] renderers = vehicle.GetComponentsInChildren<Renderer>();
            float duration = 2.5f;       
            float delayBeforeFade = 1.0f; 

            List<Material> fadeMaterials = new List<Material>();
            
            fadeMaterials.Add(vehicle.vehicleController.vehicleTurnLights._leftFrontTurnLight.material);
            fadeMaterials.Add(vehicle.vehicleController.vehicleTurnLights._leftRearTurnLight.material);
            fadeMaterials.Add(vehicle.vehicleController.vehicleTurnLights._rightFrontTurnLight.material);
            fadeMaterials.Add(vehicle.vehicleController.vehicleTurnLights._rightRearTurnLight.material);
            
            foreach (Renderer renderer in renderers)
            {
                foreach (Material mat in renderer.materials)
                {
                    SetMaterialToTransparent(mat);
                    fadeMaterials.Add(mat);
                }
            }
            
            foreach (Material mat in fadeMaterials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = 1f;
                    mat.color = c;
                }
            }

            // Ждём перед началом затухания
            float waitElapsed = 0f;
            while (waitElapsed < delayBeforeFade)
            {
                waitElapsed += Time.deltaTime;
                yield return null;
            }

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float alpha = Mathf.Lerp(1f, 0f, t); 

                foreach (Material mat in fadeMaterials)
                {
                    if (mat.HasProperty("_Color"))
                    {
                        Color c = mat.color;
                        c.a = alpha;
                        mat.color = c;
                    }
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            foreach (Material mat in fadeMaterials)
            {
                if (mat.HasProperty("_Color"))
                {
                    Color c = mat.color;
                    c.a = 0f;
                    mat.color = c;
                }
                SetMaterialToOpaque(mat);
            }
        }

        private static void SetMaterialToTransparent(Material mat)
        {
            mat.SetFloat("_Mode", 3); // 3 = Transparent
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            mat.SetInt("_ZWrite", 0);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        }

        private static void SetMaterialToOpaque(Material mat)
        {
            mat.SetFloat("_Mode", 0); // 0 = Opaque
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            mat.SetInt("_ZWrite", 1);
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHABLEND_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            mat.renderQueue = -1;
        }
    }
}