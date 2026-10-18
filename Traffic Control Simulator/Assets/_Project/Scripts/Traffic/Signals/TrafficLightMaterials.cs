using UnityEngine;

[DisallowMultipleComponent]
public class TrafficLightMaterials : MonoBehaviour
{
    [Header("Traffic Light Defaul Materials")]
    public Material DefaultRedMaterial;
    public Material DefaultYellowMaterial;
    public Material DefaultGreenMaterial;

    [Header("Traffic Light Emission Materials")]
    public Material EmissionRedMaterial;
    public Material EmissionYellowMaterial;
    public Material EmissionGreenMaterial;
    
    [Header("Traffic Light UI Materials")]
    public Color RedColor;
    public Color YellowColor;
    public Color GreenColor;
}
