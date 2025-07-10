using Gley.TrafficSystem.Internal;
using Gley.UrbanAssets.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class LayerSetupWindow : SetupWindowBase
    {
        private LayerSetup layerSetup;


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            layerSetup = LayerOperations.LoadOrCreateLayers<LayerSetup>(Internal.Constants.layerPath);
            return base.Initialize(windowProperties, window);
        }


        protected override void TopPart()
        {
            layerSetup.roadLayers = LayerMaskField(new GUIContent("Road Layers", "Vehicle wheels will collide only with these layers"), layerSetup.roadLayers);
            layerSetup.trafficLayers = LayerMaskField(new GUIContent("Traffic Layers", "All traffic vehicles should be on this layer"), layerSetup.trafficLayers);
            layerSetup.buildingsLayers = LayerMaskField(new GUIContent("Buildings Layers", "Vehicles will try to avoid objects on these layers"), layerSetup.buildingsLayers);
            layerSetup.obstaclesLayers = LayerMaskField(new GUIContent("Obstacle Layers", "Vehicles will stop when objects on these layers are seen"), layerSetup.obstaclesLayers);
            layerSetup.playerLayers = LayerMaskField(new GUIContent("Player Layers", "Vehicles will stop when objects on these layers are seen"), layerSetup.playerLayers);

            EditorGUILayout.Space();
            if (GUILayout.Button("Open Tags and Layers Settings"))
            {
                SettingsService.OpenProjectSettings("Project/Tags and Layers");
            }

            base.TopPart();
        }


        private LayerMask LayerMaskField(GUIContent label, LayerMask layerMask)
        {
            LayerMask tempMask = EditorGUILayout.MaskField(label,InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMask), InternalEditorUtility.layers);
            layerMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
            return layerMask;
        }


        public override void DestroyWindow()
        {
            layerSetup.edited = true;
            EditorUtility.SetDirty(layerSetup);
            AssetDatabase.SaveAssets();
            SettingsWindow.UpdateLayers();
            base.DestroyWindow();
        }
    }
}