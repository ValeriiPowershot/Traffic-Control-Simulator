using Gley.UrbanAssets.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Gley.TrafficSystem.Editor
{
    public class VehicleTypesWindow : SetupWindowBase
    {
        private readonly float scrollAdjustment = 205;
        private string errorText;
        private List<string> vehicleCategories = new List<string>();


        public override ISetupWindow Initialize(WindowProperties windowProperties, SettingsWindowBase window)
        {
            errorText = "";
            LoadVehicles();
            return base.Initialize(windowProperties, window);
        }


        private void LoadVehicles()
        {
            var allCarTypes = Enum.GetValues(typeof(VehicleTypes)).Cast<VehicleTypes>();
            foreach (VehicleTypes car in allCarTypes)
            {
                vehicleCategories.Add(car.ToString());
            }
        }


        protected override void TopPart()
        {
            base.TopPart();
            EditorGUILayout.LabelField("Vehicle types are used to limit vehicle movement.\n" +
                "You can use different vehicle types to restrict the access of different type of vehicles in some areas.");
        }


        protected override void ScrollPart(float width, float height)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false, GUILayout.Width(width - SCROLL_SPACE), GUILayout.Height(height - scrollAdjustment));

            for (int i = 0; i < vehicleCategories.Count; i++)
            {
                GUILayout.BeginHorizontal();
                vehicleCategories[i] = EditorGUILayout.TextField(vehicleCategories[i]);
                vehicleCategories[i] = Regex.Replace(vehicleCategories[i], @"^[\d-]*\s*", "");
                vehicleCategories[i] = vehicleCategories[i].Replace(" ", "");
                vehicleCategories[i] = vehicleCategories[i].Trim();
                if (GUILayout.Button("Remove"))
                {
                    vehicleCategories.RemoveAt(i);
                }
                GUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add vehicle category"))
            {
                vehicleCategories.Add("");
            }

            GUILayout.EndScrollView();
        }


        protected override void BottomPart()
        {
            GUILayout.Label(errorText);
            if (GUILayout.Button("Save"))
            {
                if (CheckForNull() == false)
                {
                    errorText = "Success";
                    Save();
                }
            }
            EditorGUILayout.Space();
            base.BottomPart();
        }


        private void Save()
        {
            FileCreator.CreateAgentTypesFile<VehicleTypes>(vehicleCategories, Internal.Constants.GLEY_TRAFFIC_SYSTEM, Internal.Constants.trafficNamespace, Internal.Constants.agentTypesPath);
        }


        private bool CheckForNull()
        {
            for (int i = 0; i < vehicleCategories.Count - 1; i++)
            {
                for (int j = i + 1; j < vehicleCategories.Count; j++)
                {
                    if (vehicleCategories[i] == vehicleCategories[j])
                    {
                        errorText = vehicleCategories[i] + " Already exists. No duplicates allowed";
                        return true;
                    }
                }
            }
            for (int i = 0; i < vehicleCategories.Count; i++)
            {
                if (string.IsNullOrEmpty(vehicleCategories[i]))
                {
                    errorText = "Car category cannot be empty! Please fill all of them";
                    return true;
                }
            }
            return false;
        }
    }
}
