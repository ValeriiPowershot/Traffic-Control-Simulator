using UnityEngine;

[DisallowMultipleComponent]
public class TrafficLightColorSwitcher : MonoBehaviour
{
    [SerializeField] private TrafficLightMaterials _trafficLightMaterials;
    
    [Header("Traffic Light")]
    public MeshRenderer RedMeshRenderer;
    public MeshRenderer YellowMeshRenderer;
    public MeshRenderer GreenMeshRenderer;

    public void ChangeToRed()
    {
        RedMeshRenderer.material = _trafficLightMaterials.EmissionRedMaterial;
        YellowMeshRenderer.material = _trafficLightMaterials.DefaultGreenMaterial;
        GreenMeshRenderer.material = _trafficLightMaterials.DefaultGreenMaterial;
    }
    
    public void ChangeToYellow()
    {
        RedMeshRenderer.material = _trafficLightMaterials.DefaultRedMaterial;
        YellowMeshRenderer.material = _trafficLightMaterials.EmissionYellowMaterial;
        GreenMeshRenderer.material = _trafficLightMaterials.DefaultGreenMaterial;
    }
    
    public void ChangeToGreen()
    {
        RedMeshRenderer.material = _trafficLightMaterials.DefaultRedMaterial;
        YellowMeshRenderer.material = _trafficLightMaterials.DefaultGreenMaterial;
        GreenMeshRenderer.material = _trafficLightMaterials.EmissionGreenMaterial;
    }
}
