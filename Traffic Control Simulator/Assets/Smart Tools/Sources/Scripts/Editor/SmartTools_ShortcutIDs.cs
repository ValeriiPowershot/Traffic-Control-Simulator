using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShortcutManagement;

namespace SmartTools
{
    public partial class SmartTools
    {
        const string id_Category = "SmartTools/";

        const string id_ShowGrid                                = id_Category + "Show Grid";
        const string id_ShowSettings                            = id_Category + "Show Settings";
        const string id_CycleGridSizePresets                    = id_Category + "Cycle GridSize Presets";
        const string id_TogglePivotAndCenterMode                = id_Category + "Toggle Pivot and Center Mode";
        const string id_ToggleRotationAngleAndMoveIncrement     = id_Category + "Toggle Rotation Angle and Move Increment";
        const string id_ToggleMoveIncrement                     = id_Category + "Toggle Move Increment";
        const string id_ToggleRotationAngle                     = id_Category + "Toggle Rotation Angle";
        const string id_SwitchRotationAxis                      = id_Category + "Switch Rotation Axis";
        const string id_MoveLeft                                = id_Category + "Move Left";
        const string id_MoveRight                               = id_Category + "Move Right";
        const string id_MoveForward                             = id_Category + "Move Forward";
        const string id_MoveBack                                = id_Category + "Move Back";
        const string id_MoveUp                                  = id_Category + "Move Up";
        const string id_MoveDown                                = id_Category + "Move Down";
        const string id_RotateLeft                              = id_Category + "Rotate Left";
        const string id_RotateRight                             = id_Category + "Rotate Right";
        const string id_Duplicate                               = id_Category + "Duplicate";
        const string id_Focus                                   = id_Category + "Focus";
        const string id_SnapTransform                           = id_Category + "Snap Transform";
        const string id_ResetTransform                          = id_Category + "Reset Transform";
        const string id_SnapPosition                            = id_Category + "Snap Position";
        const string id_ResetPosition                           = id_Category + "Reset Position";
        const string id_SnapRotation                            = id_Category + "Snap Rotation";
        const string id_ResetRotation                           = id_Category + "Reset Rotation";
        const string id_SnapScale                               = id_Category + "Snap Scale";
        const string id_ResetScale                              = id_Category + "Reset Scale";
        const string id_CreateParentAtOrigin                    = id_Category + "Create Parent at Origin";
        const string id_CreateParentAtSelectionCenter           = id_Category + "Create Parent at Selection Center";
        const string id_MoveParentToSelectionCenter             = id_Category + "Move Parent to Selection Center";
        const string id_UnparentSelection                       = id_Category + "Unparent Selection";
        const string id_UnparentSelectionUpInHierarchy          = id_Category + "Unparent Selection (Up in Hierarchy)";
        const string id_SelectParents                           = id_Category + "Select Parents";
        const string id_SelectChildren                          = id_Category + "Select Children";
        const string id_ChildCompensation                       = id_Category + "Child Compensation";
        const string id_PreviousSelection                       = id_Category + "Previous Selection";
        const string id_NextSelection                           = id_Category + "Next Selection";
        const string id_LoadViewAndSelection1                   = id_Category + "Load View And Selection 1";
        const string id_LoadViewAndSelection2                   = id_Category + "Load View And Selection 2";
        const string id_LoadViewAndSelection3                   = id_Category + "Load View And Selection 3";
        const string id_LoadViewAndSelection4                   = id_Category + "Load View And Selection 4";
        const string id_LoadViewAndSelection5                   = id_Category + "Load View And Selection 5";
        const string id_LoadViewAndSelection6                   = id_Category + "Load View And Selection 6";
        const string id_LoadViewAndSelection7                   = id_Category + "Load View And Selection 7";
        const string id_LoadViewAndSelection8                   = id_Category + "Load View And Selection 8";
        const string id_LoadViewAndSelection9                   = id_Category + "Load View And Selection 9";
        const string id_LoadViewAndSelection10                  = id_Category + "Load View And Selection 10";
        const string id_LoadViewAndSelection11                  = id_Category + "Load View And Selection 11";
        const string id_LoadViewAndSelection12                  = id_Category + "Load View And Selection 12";

        static string GetShortcut(string id = "")
        {
            if (id == "") return "IMPLEMENT!!!";
            var binding = ShortcutManager.instance.GetShortcutBinding(id).ToString();
            return binding != "" ? binding : "None"; 
        }
    }
}
