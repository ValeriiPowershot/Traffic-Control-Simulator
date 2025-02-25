using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.ShortcutManagement;

namespace SmartTools
{
    public partial class SmartTools
    {
        static public string tt_Main;
        static public string tt_Grid;
        static public string tt_Settings;
        static public string tt_GridSize;
        static public string tt_Increments;
        static public string tt_IncSwitch;
        static public string tt_Degrees;
        static public string tt_Pivot;
        static public string tt_DegSwitch;
        static public string tt_RotAxisX;
        static public string tt_RotAxisY;
        static public string tt_RotAxisZ;
        static public string tt_RotateL;
        static public string tt_MoveUp;
        static public string tt_RotateR;
        static public string tt_MoveL;
        static public string tt_MoveD;
        static public string tt_MoveR;
        static public string tt_MoveF;
        static public string tt_MoveB;
        static public string tt_Duplicate;
        static public string tt_Focus;
        static public string tt_Snap;
        static public string tt_SnapPos;
        static public string tt_SnapPosX;
        static public string tt_SnapPosY;
        static public string tt_SnapPosZ;
        static public string tt_SnapRot;
        static public string tt_SnapRotX;
        static public string tt_SnapRotY;
        static public string tt_SnapRotZ;
        static public string tt_SnapScl;
        static public string tt_SnapSclX;
        static public string tt_SnapSclY;
        static public string tt_SnapSclZ;
        static public string tt_CComp;
        static public string tt_Parent;
        static public string tt_SelParents;
        static public string tt_SelChildren;
        static public string tt_Unparent;
        static public string tt_PrevSel;
        static public string tt_NextSel;

        static public string tt_ViewDependency;
        static public string tt_MoveAreaSize;
        static public string tt_HorizontalMode;

        static public void InitToolTips()
        {
            tt_Main = "Double-Click this logo to open the manual.\n\nRight-Click to go to the SnazzyGrid forum.\n\nHi, SnazzyGrid is designed to save you some time and make the work with Unity generally more enjoyable. " +
                "If you find SnazzyTools helpful we would appreciate if you spend some of your saved time to leave a rating/review on the Assetstore. " +
                "Also, if you have ideas on how to improve SnazzyTools please feel encouraged to visit us on the Unity Forum thread (Link in Settings/Help). We wish you a snazzy workflow and good luck with your work.";
            tt_Grid = 
                "<b><color=white>Left-Click:</color></b> Shows/hides the grid.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_ShowGrid) + "</color>\n\n" +
                 "<b><color=white>Right-Click:</color></b> Visually offsets the grid by a half unit.";
            tt_Settings =
                "<b><color=white>Left-Click:</color></b> Opens the Quick Guide.\n" +
                 "<b><color=white>Right-Click:</color></b> Opens the Documentation.\n" +
                 "<b><color=white>Middle-Click:</color></b> Opens the Settings.";
            tt_GridSize = 
                "<b><color=white>Left-Click:</color></b> Cycles through some grid size presets.\n" +
                "<b><color=white>Right-Click:</color></b> Cycles backwards.";
            tt_Increments = 
                "Defines the number of grid units selected objects move when using the move buttons or hotkeys.";
            tt_IncSwitch = 
                "<b><color=white>Left-Click:</color></b> Toggles between the first and second move increment.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_ToggleRotationAngleAndMoveIncrement) + "</color> (Also toggles the rotation angle)\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_ToggleMoveIncrement) + "</color> (Toggles move increment only)\n\n" +
                "<b><color=white>Right-Click:</color></b> Cycles forward through preset values.\n" +
                "<b><color=white>Middle-Click:</color></b> Cycles backwards.";
            tt_Degrees = 
                "Defines the number of degrees selected objects rotate when using the rotate buttons or hotkeys.";
            tt_Pivot = 
                "When enabled, objects rotate around their individual pivots using the rotation buttons or hotkeys.";
            tt_DegSwitch = 
                "<b><color=white>Left-Click:</color></b> Toggles between the first and second rotation angle.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_ToggleRotationAngleAndMoveIncrement) + "</color> (Also toggles the move increment)\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_ToggleRotationAngle) + "</color> (Toggles rotation angle only)\n\n" +
                "<b><color=white>Right-Click:</color></b> Cycles forward through preset values.\n" +
                "<b><color=white>Middle-Click:</color></b> Cycles backwards.";
            tt_RotAxisX = 
                "<b><color=white>Left-Click:</color></b> Selects the rotation axis used by the rotation buttons and hotkeys.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_SwitchRotationAxis) + "</color> (cycles through axes)";
            tt_RotAxisY = 
                "<b><color=white>Left-Click:</color></b> Selects the rotation axis used by the rotation buttons and hotkeys.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_SwitchRotationAxis) + "</color> (cycles through axes)";
            tt_RotAxisZ = 
                "<b><color=white>Left-Click:</color></b> Selects the rotation axis used by the rotation buttons and hotkeys.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_SwitchRotationAxis) + "</color> (cycles through axes)";
            tt_RotateL = 
                "<b><color=white>Left-Click:</color></b> Rotates the selected objects counterclockwise using the selected axis and rotation amount.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_RotateLeft) + "</color>";
            tt_MoveUp = 
                "<b><color=white>Left-Click:</color></b> Moves the selected objects up.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_MoveUp) + "</color>";
            tt_RotateR = 
                "<b><color=white>Left-Click:</color></b> Rotates the selected objects clockwise using the selected axis and rotation amount.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_RotateRight) + "</color>";
            tt_MoveL = 
                "<b><color=white>Left-Click:</color></b> Moves the selected objects to the left.\n" +
               "Hotkey: <color=#E5C100>" + GetShortcut(id_MoveLeft) + "</color>";
            tt_MoveD = 
                "<b><color=white>Left-Click:</color></b> Moves the selected objects down.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_MoveDown) + "</color>";
            tt_MoveR = 
                "<b><color=white>Left-Click:</color></b> Moves the selected objects to the right.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_MoveRight) + "</color>";
            tt_MoveF = 
                "<b><color=white>Left-Click:</color></b> Moves the selected objects forward.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_MoveForward) + "</color>";
            tt_MoveB = 
                "<b><color=white>Left-Click:</color></b> Moves the selected objects back.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_MoveBack) + "</color>";
            tt_Duplicate = 
                "<b><color=white>Left-Click:</color></b> Duplicates the selected objects.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_Duplicate) + "</color>";
            tt_Focus = 
                "<b><color=white>Left-Click:</color></b> Toggles focus mode, keeping the scene view focused on the selected objects.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_Focus) + "</color>\n\n" +
                "(Panning the scene view automatically ends focus mode)";
            tt_Snap = 
                "<b><color=white>Left-Click:</color></b> Toggle snapping on or off.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_SnapTransform) + "</color> (Tap to toggle, hold to temporarily toggle)\n\n" +
                "<b><color=white>Middle-Click:</color></b> Snap the transforms of the selected objects according to the snap settings.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_SnapTransform) + "</color> (Double-tap)\n\n" +
                "<b><color=white>Right-Click:</color></b> Reset the transforms of the selected objects.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_ResetTransform) + "</color>";
            tt_SnapPos = 
                "<b><color=white>Left-Click:</color></b> Enables/disables position snapping.\n" +
                 "Hotkey: <color=#E5C100>" + GetShortcut(id_SnapPosition) + "</color> (Tap to toggle, hold to temporarily toggle)\n\n" +
                 "<b><color=white>Middle-Click:</color></b> Snaps the position of the selected objects according to the selected axis.\n" +
                 "Hotkey: <color=#E5C100>" + GetShortcut(id_SnapPosition) + "</color> (Double-tap)\n\n" +
                 "<b><color=white>Right-Click:</color></b> Resets the position of the selected objects.\n" +
                 "Hotkey: <color=#E5C100>" + GetShortcut(id_ResetPosition) + "</color>";
            tt_SnapPosX = 
                "<b><color=white>Left-Click:</color></b> Enables/disables position snapping for the X axis.\n" +
                "<b><color=white>Middle-Click:</color></b> Snaps the X position of the selected objects.\n" +
                "<b><color=white>Right-Click:</color></b> Resets only the X position of the selected objects.";
            tt_SnapPosY = 
                "<b><color=white>Left-Click:</color></b> Enables/disables position snapping for the Y axis.\n" +
                "<b><color=white>Middle-Click:</color></b> Snaps the Y position of the selected objects.\n" +
                "<b><color=white>Right-Click:</color></b> Resets only the Y position of the selected objects.";
            tt_SnapPosZ = 
                "<b><color=white>Left-Click:</color></b> Enables/disables position snapping for the Z axis.\n" +
                "<b><color=white>Middle-Click:</color></b> Snaps the Z position of the selected objects.\n" +
                "<b><color=white>Right-Click:</color></b> Resets only the Z position of the selected objects.";
            tt_SnapRot = 
                "<b><color=white>Left-Click:</color></b> Enables/disables rotation snapping.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_SnapRotation) + "</color> (Tap to toggle, hold to temporarily toggle)\n\n" +
                "<b><color=white>Middle-Click:</color></b> Snaps the rotation of the selected objects according to the selected axis.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_SnapRotation) + "</color> (Double-tap)\n\n" +
                "<b><color=white>Right-Click:</color></b> Resets the rotation of the selected objects.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_ResetRotation) + "</color>";
            tt_SnapRotX = 
                "<b><color=white>Left-Click:</color></b> Enables/disables rotation snapping for the X axis.\n" +
                "<b><color=white>Middle-Click:</color></b> Snaps the X rotation of the selected objects.\n" +
                "<b><color=white>Right-Click:</color></b> Resets only the X rotation of the selected objects.";
            tt_SnapRotY = 
                "<b><color=white>Left-Click:</color></b> Enables/disables rotation snapping for the Y axis.\n" +
                "<b><color=white>Middle-Click:</color></b> Snaps the Y rotation of the selected objects.\n" +
                "<b><color=white>Right-Click:</color></b> Resets only the Y rotation of the selected objects.";
            tt_SnapRotZ = 
                "<b><color=white>Left-Click:</color></b> Enables/disables rotation snapping for the Z axis.\n" +
                "<b><color=white>Middle-Click:</color></b> Snaps the Z rotation of the selected objects.\n" +
                "<b><color=white>Right-Click:</color></b> Resets only the Z rotation of the selected objects.";
            tt_SnapScl = 
                "<b><color=white>Left-Click:</color></b> Enables/disables scale snapping.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_SnapScale) + "</color> (Tap to toggle, hold to temporarily toggle)\n\n" +
                "<b><color=white>Middle-Click:</color></b> Snaps the scale of the selected objects according to the selected axis.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_SnapScale) + "</color> (Double-tap)\n\n" +
                "<b><color=white>Right-Click:</color></b> Resets the scale of the selected objects.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_ResetScale) + "</color>\n\n" +
                "<b><color=white>Shift Click:</color></b> Flips the scale of the selected objects.\n\n" +
                "<b><color=white>Control Left/Right-Click:</color></b> Doubles/halves the scale of the selected objects.";
            tt_SnapSclX = 
                "<b><color=white>Left-Click:</color></b> Enables/disables scale snapping for the X axis.\n" +
                "<b><color=white>Middle-Click:</color></b> Snaps the X scale of the selected objects.\n" +
                "<b><color=white>Right-Click:</color></b> Resets only the X scale of the selected objects.\n\n" +
                "<b><color=white>Shift Click:</color></b> Flips the X scale of the selected objects.\n\n" +
                "<b><color=white>Control Left/Right-Click:</color></b> Doubles/halves the X scale of the selected objects.";
            tt_SnapSclY = 
                "<b><color=white>Left-Click:</color></b> Enables/disables scale snapping for the Y axis.\n" +
                "<b><color=white>Middle-Click:</color></b> Snaps the Y scale of the selected objects.\n" +
                "<b><color=white>Right-Click:</color></b> Resets only the Y scale of the selected objects.\n\n" +
                "<b><color=white>Shift Click:</color></b> Flips the Y scale of the selected objects.\n\n" +
                "<b><color=white>Control Left/Right-Click:</color></b> Doubles/halves the Y scale of the selected objects.";
            tt_SnapSclZ = 
                "<b><color=white>Left-Click:</color></b> Enables/disables scale snapping for the Z axis.\n" +
                "<b><color=white>Middle-Click:</color></b> Snaps the Z scale of the selected objects.\n" +
                "<b><color=white>Right-Click:</color></b> Resets only the Z scale of the selected objects.\n\n" +
                "<b><color=white>Shift Click:</color></b> Flips the Z scale of the selected objects.\n\n" +
                "<b><color=white>Control Left/Right-Click:</color></b> Doubles/halves the Z scale of the selected objects.";
            tt_CComp = 
                "<b><color=white>Left-Click:</color></b> When enabled, the children of the selected objects won't be affected by moving, rotating, or scaling the parent.\n" +
               "Hotkey: <color=#E5C100>" + GetShortcut(id_ChildCompensation) + "</color>\n\n" +
               "<b>Note:</b> Be aware that non-uniform scaling can have unwanted effects on child transforms. This is not a limitation of this tool but how scaling is handled in Unity in general.";
            tt_Parent = 
                "<b><color=white>Left-Click:</color></b> Creates a new parent for the selected objects at the scene origin.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_CreateParentAtOrigin) + "</color>\n\n" +
                "<b><color=white>Right-Click:</color></b> Creates a new parent for the selected objects at the center of the selection.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_CreateParentAtSelectionCenter) + "</color>\n\n" +
                "<b><color=white>Middle-Click:</color></b> Moves the parent of the selected objects to the scene origin.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_MoveParentToSelectionCenter) + "</color>";
            tt_SelParents = 
                "<b><color=white>Left-Click:</color></b> Selects the parents of the selected objects.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_SelectParents) + "</color>";
            tt_SelChildren = 
                "<b><color=white>Left-Click:</color></b> Selects the children of the selected objects.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_SelectChildren) + "</color>";
            tt_Unparent = 
                "<b><color=white>Left-Click:</color></b> Unparents the selected objects completely, parenting them to the scene root.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_UnparentSelection) + "</color>\n\n" +
                "<b><color=white>Right-Click:</color></b> Unparents the selected objects by one level, making their parent a sibling.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_UnparentSelectionUpInHierarchy) + "</color>";
            tt_PrevSel = 
                "<b><color=white>Left-Click:</color></b> Cycles your selections backward.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_PreviousSelection) + "</color>\n\n" +
                "Note: Selection history is not retained between editor sessions.";
            tt_NextSel = 
                "<b><color=white>Left-Click:</color></b> Cycles your selections forward.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_NextSelection) + "</color>\n\n" +
                "Note: Selection history is not retained between editor sessions.";
            tt_ViewDependency =
                "<b>View Dependency</b>\n\n" +
                "Controls the view dependency of the grid, making the camera-facing plane most visible and causing the other planes to fade out.\n\n" +
                "<b><color=white>Right-Click:</color></b> Resets the slider to its default value.";
            tt_MoveAreaSize =
                "<b>Opacity</b>\n\n" +
                "Allows you to add opacity to the grid. This can help you better see where an object is placed relative to its surroundings.\n\n" +
                "<b><color=white>Right-Click:</color></b> Resets the slider to its default value."; 
            tt_HorizontalMode =
                "<b>Movement Orientation</b>\n\n" +
                "This slider affects how the directions of the move buttons and hotkeys are calculated. " +
                "Movement is always camera-relative, meaning 'right' means right relative to the camera, snapped to the nearest aligning axis. " +
                "Moving the slider to the left will orient movement on a horizontal plane, while moving the slider to the right will orient movement on a vertical plane.\n\n" +
                "You can see the point at which the system switches by looking up and down with the scene camera while observing the slider knob. It will turn green when the switch happens.\n\n" +
                "<b><color=white>Right-Click:</color></b> Resets the slider to its default value.";
            //CopyTooltipsToClipboard();
        }

        public static void CopyTooltipsToClipboard()
        {
            // Combine all the static string variables after stripping out the rich text tags
            string combinedText = string.Join("\n\n\n",
                RemoveRichText(tt_Main),
                RemoveRichText(tt_Grid),
                RemoveRichText(tt_GridSize),
                RemoveRichText(tt_Increments),
                RemoveRichText(tt_IncSwitch),
                RemoveRichText(tt_Degrees),
                RemoveRichText(tt_Pivot),
                RemoveRichText(tt_DegSwitch),
                RemoveRichText(tt_RotAxisX),
                RemoveRichText(tt_RotAxisY),
                RemoveRichText(tt_RotAxisZ),
                RemoveRichText(tt_RotateL),
                RemoveRichText(tt_MoveUp),
                RemoveRichText(tt_RotateR),
                RemoveRichText(tt_MoveL),
                RemoveRichText(tt_MoveD),
                RemoveRichText(tt_MoveR),
                RemoveRichText(tt_MoveF),
                RemoveRichText(tt_MoveB),
                RemoveRichText(tt_Duplicate),
                RemoveRichText(tt_Focus),
                RemoveRichText(tt_Snap),
                RemoveRichText(tt_SnapPos),
                RemoveRichText(tt_SnapPosX),
                RemoveRichText(tt_SnapPosY),
                RemoveRichText(tt_SnapPosZ),
                RemoveRichText(tt_SnapRot),
                RemoveRichText(tt_SnapRotX),
                RemoveRichText(tt_SnapRotY),
                RemoveRichText(tt_SnapRotZ),
                RemoveRichText(tt_SnapScl),
                RemoveRichText(tt_SnapSclX),
                RemoveRichText(tt_SnapSclY),
                RemoveRichText(tt_SnapSclZ),
                RemoveRichText(tt_CComp),
                RemoveRichText(tt_Parent),
                RemoveRichText(tt_SelParents),
                RemoveRichText(tt_SelChildren),
                RemoveRichText(tt_Unparent),
                RemoveRichText(tt_PrevSel),
                RemoveRichText(tt_NextSel),
                RemoveRichText(tt_ViewDependency),
                RemoveRichText(tt_MoveAreaSize),
                RemoveRichText(tt_HorizontalMode)
            );

            // Copy the combined text to the clipboard
            GUIUtility.systemCopyBuffer = combinedText;

            Debug.Log("Tooltips copied to clipboard without rich text.");
        }

        private static string RemoveRichText(string input)
        {
            // Use a regex to remove any rich text tags (e.g., <b>, <i>, <color>, etc.)
            return System.Text.RegularExpressions.Regex.Replace(input, "<.*?>", string.Empty);
        }

        static string PresetButtonTooltip(int index)
        {
            int number = index + 1;
            string s =
                "<b>View/Selection Set " + number + "</b>\n\n" +
            
                "<b><color=white>Left-Click:</color></b> Load both view & selection.\n" +
                "Hotkey: <color=#E5C100>" + GetShortcut(id_LoadViewAndSelection1.Replace("1", number.ToString())) + "</color>\n" +
                "<b><color=white>Right-Click:</color></b> Load view only.\n" +
                "<b><color=white>Middle-Click:</color></b> Load selection only.\n\n" +
            
                "<b><color=white>Shift Left-Click:</color></b> Save both view & selection.\n" +
                "<b><color=white>Shift Right-Click:</color></b> Save view only.\n" +
                "<b><color=white>Shift Middle-Click:</color></b> Save selection only.\n\n" +

                "<b><color=white>Control Left-Click:</color></b> Clear both view & selection.\n" +
                "<b><color=white>Control Right-Click:</color></b> Clear view only.\n" +
                "<b><color=white>Control Middle-Click:</color></b> Clear selection only.";
            return s;
        }
    }
}
