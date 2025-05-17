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
            Renderer renderers = vehicle.GetComponent<Renderer>();
            float duration = 2.5f;
            float elapsed = 0f;

            List<Material> fadeMaterials = new List<Material>();
                
            foreach (var mat in renderers.materials)
            {
                fadeMaterials.Add(mat);
            }
            
            while (elapsed < duration)
            {
                float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

                foreach (var mat in fadeMaterials)
                {
                    Color c = mat.color;
                    c.a = alpha;
                    mat.color = c;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }
            foreach (var mat in fadeMaterials)
            {
                Color c = mat.color;
                c.a = 1;
                mat.color = c;
            }
        }
    }
}