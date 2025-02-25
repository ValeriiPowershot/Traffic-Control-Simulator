using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.Events;
using UnityEditor.ShortcutManagement;

namespace SmartTools
{ 
    [Overlay(typeof(SceneView), "SmartTools", "Smart", false, defaultDockZone = DockZone.Floating )]
    public partial class SmartTools : Overlay
    {
        /////////////////////////////////////////////////////////////////////////////////////
        // GRID SECTION =====================================================================
        /////////////////////////////////////////////////////////////////////////////////////
        #region Grid Section
        static void On_Sld_HorizontalMode(ChangeEvent<float> evt)
        {
            float sign = Mathf.Sign(sld_HorizontalMode.value);
            horizontalMode = (sld_HorizontalMode.value * sld_HorizontalMode.value) * sign;
            //sld_HorizontalMode.Q("unity-dragger").style.backgroundColor = GetHorizontalMode() ? Color.green : new Color(0.6f, 0.6f, 0.6f, 1);
        }

        static void On_Sld_ViewDependency(ChangeEvent<float> evt)
        {
            viewDependency = sld_ViewDependency.value;
        }
        static void On_Sld_MoveAreaSize(ChangeEvent<float> evt)
        {
            moveAreaSize = sld_MoveAreaSize.value;
        }



        [Shortcut(id_ShowGrid, KeyCode.G, ShortcutModifiers.Shift)]
        static void On_btn_ShowGrid()
        {
            if (!IsDisplayed()) return;

            var e = Event.current;

            if (e.button == 0) FlipToggle(ref showGrid, tgl_GridVisiblity);
            else if (e.button == 1) offsetHalfGrid = !offsetHalfGrid;


            bool shift = e.modifiers == EventModifiers.Shift;
            bool control = e.modifiers == EventModifiers.Control;
            bool alt = e.modifiers == EventModifiers.Alt;

            if (e.modifiers == EventModifiers.Alt && e.button == 2)
            {
                bool isHidden = sceneData.gameObject.hideFlags == HideFlags.HideInHierarchy;
                Debug.Log(isHidden ? "Unhiding Scene Data" : "Hiding Scene Data", sceneData.gameObject);
                sceneData.gameObject.hideFlags = isHidden ? HideFlags.None : HideFlags.HideInHierarchy; 
            }
        }


    

        [Shortcut(id_ShowSettings)]
        static void On_btn_ShowSettings()
        {
            if (!IsDisplayed()) return;

            var button = Event.current.button;
            if (button == 0)
            {
                FlipToggle(ref showSettings, tgl_Settings);
                if (showSettings) { root.style.width = 64 + helpAreaWidth; }
                else { root.style.width = 64; }
            }
            else if (button == 1)
            {
                Application.OpenURL("https://docs.google.com/document/d/1JQmqUnR17zHTi81VqtJuYYfA7gpuc0vVLCchX7QOrvE/edit?usp=drive_link");
            }
            else if (button == 2)
            {
                Selection.activeObject = settings;
            }
        }


        [Shortcut(id_CycleGridSizePresets)]
        static void On_btn_GridSize()
        {
            if (!IsDisplayed()) return;
            HandleFloatPresetButtons(settings.gridSizePresets, flf_GridSize, ref gridSize, 0, 1);
            UpdateMoveDistance();
        }
        static void On_flf_GridSize(ChangeEvent<float> evt)
        {
            FloatField flf = (FloatField)evt.target;
            var value = flf.value;
            if (value < 0.001f) value = 0.001f;
            flf.value = gridSize = value;
            UpdateMoveDistance();
        }
        #endregion



        /////////////////////////////////////////////////////////////////////////////////////
        // INCREMENT AND ANGLE SECTION ======================================================
        /////////////////////////////////////////////////////////////////////////////////////
        #region Increment and Angle Section
        [Shortcut(id_TogglePivotAndCenterMode)]
        static void On_btn_Pivot()
        {
            if (!IsDisplayed()) return;
            FlipToggle(ref pivot, tgl_Pivot);
        }


        [Shortcut(id_ToggleRotationAngleAndMoveIncrement, KeyCode.KeypadPeriod, ShortcutModifiers.None)]
        static void On_CompositeToggle()
        {
            if (!IsDisplayed()) return;
            On_btn_Increment();
            On_btn_Angle();
        }

        [Shortcut(id_ToggleMoveIncrement)]
        static void On_btn_Increment()
        {
            if (!IsDisplayed()) return;
            Toggle2Way(ref moveIncrement, tgl_IncrementA, tgl_IncrementB);
            UpdateMoveDistance();
        }
        static void On_btn_IncrementA()
        {
            if (!IsDisplayed()) return;
            if (Event.current.button == 0) On_btn_Increment();
            else HandleFloatPresetButtons(settings.incrementPresets, flf_IncrementA, ref moveIncrementA, 1, 2);
            UpdateMoveDistance();
        }
        static void On_btn_IncrementB()
        {
            if (Event.current.button == 0) On_btn_Increment();
            else HandleFloatPresetButtons(settings.incrementPresets, flf_IncrementB, ref moveIncrementB, 1, 2);
            UpdateMoveDistance();
        }
        static void On_flf_IncrementA(ChangeEvent<float> evt)
        {
            HandleFloatFields(flf_IncrementA, ref moveIncrementA);
            UpdateMoveDistance();
        }
        static void On_flf_IncrementB(ChangeEvent<float> evt)
        {
            HandleFloatFields(flf_IncrementB, ref moveIncrementB);
            UpdateMoveDistance();
        }




        [Shortcut(id_ToggleRotationAngle)]
        static void On_btn_Angle()
        {
            if (!IsDisplayed()) return;
            Toggle2Way(ref rotIncrement, tgl_AngleA, tgl_AngleB);
            UpdateRotAmount();
        }
        static void On_btn_AngleA()
        {
            if (Event.current.button == 0) On_btn_Angle();
            else HandleFloatPresetButtons(settings.anglePresets, flf_AngleA, ref rotIncrementA, 1, 2);
            UpdateRotAmount();
        }
        static void On_btn_AngleB()
        {
            if (Event.current.button == 0) On_btn_Angle();
            else HandleFloatPresetButtons(settings.anglePresets, flf_AngleB, ref rotIncrementB, 1, 2);
            UpdateRotAmount();
        }
        static void On_flf_AngleA(ChangeEvent<float> evt)
        {
            HandleFloatFields(flf_AngleA, ref rotIncrementA, 1, 180);
            UpdateRotAmount();
        }
        static void On_flf_AngleB(ChangeEvent<float> evt)
        {
            HandleFloatFields(flf_AngleB, ref rotIncrementB, 1, 180);
            UpdateRotAmount();
        }



        [Shortcut(id_SwitchRotationAxis, KeyCode.KeypadDivide, ShortcutModifiers.None)]
        static void On_ShortCut_Axis()
        {
            if (!IsDisplayed()) return;
            rotAxis++; if (rotAxis > 2) rotAxis = 0; UpdateRotAxisButtons();
        }
        static void On_btn_AxisX() { rotAxis = 0; UpdateRotAxisButtons(); }
        static void On_btn_AxisY() { rotAxis = 1; UpdateRotAxisButtons(); }
        static void On_btn_AxisZ() { rotAxis = 2; UpdateRotAxisButtons(); }
        #endregion



        /////////////////////////////////////////////////////////////////////////////////////
        // MOVE AND ROTATE TOOLS (+ Focus and Duplicate) ====================================
        /////////////////////////////////////////////////////////////////////////////////////
        #region Move and Rotate Tools
        [Shortcut(id_MoveLeft, KeyCode.Keypad4, ShortcutModifiers.None)]
        static void OnMoveLeft()
        {
            if (!IsDisplayed()) return;
            MoveSelection(1);
        }



        [Shortcut(id_MoveRight, KeyCode.Keypad6, ShortcutModifiers.None)]
        static void OnMoveRight()
        {
            if (!IsDisplayed()) return;
            MoveSelection(0);
        }



        [Shortcut(id_MoveUp, KeyCode.Keypad8, ShortcutModifiers.None)]
        static void OnMoveUp()
        {
            if (!IsDisplayed()) return;
            MoveSelection(2);
        }



        [Shortcut(id_MoveDown, KeyCode.Keypad2, ShortcutModifiers.None)]
        static void OnMoveDown()
        {
            if (!IsDisplayed()) return;
            MoveSelection(3);
        }



        [Shortcut(id_MoveForward, KeyCode.PageUp, ShortcutModifiers.None)]
        static void OnMoveForward()
        {
            if (!IsDisplayed()) return;
            MoveSelection(4);
        }



        [Shortcut(id_MoveBack, KeyCode.PageDown, ShortcutModifiers.None)]
        static void OnMoveBack()
        {
            if (!IsDisplayed()) return;
            MoveSelection(5);
        }



        [Shortcut(id_RotateLeft, KeyCode.Keypad7, ShortcutModifiers.None)]
        static void OnRotateLeft()
        {
            if (!IsDisplayed()) return;
            RotateSelection(-1);
        }



        [Shortcut(id_RotateRight, KeyCode.Keypad9, ShortcutModifiers.None)]
        static void OnRotateRight()
        {
            if (!IsDisplayed()) return;
            RotateSelection(1);
        }



        [Shortcut(id_Duplicate, KeyCode.KeypadMultiply, ShortcutModifiers.None)]
        static void OnDuplicate()
        {
            if (!IsDisplayed()) return;
            EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent("Duplicate"));
        }



        [Shortcut(id_Focus, KeyCode.Keypad5, ShortcutModifiers.None)]
        static void OnFocus()
        {
            if (!IsDisplayed()) return;
            FlipToggle(ref focus, tgl_Focus);
            // Done In OnScene Function
        }
        #endregion



        /////////////////////////////////////////////////////////////////////////////////////
        // SNAPPING =========================================================================
        /////////////////////////////////////////////////////////////////////////////////////
        #region Snap

        static void OnSnap()
        {
            var button = Event.current.button;
            if (button == 0) //LMB
            {
                FlipToggle(ref snap, tgl_Snap); HandleSnap();
            }
            else if (button == 1) // RMB
            {
                ResetTransform(true);
            }
            else if (button == 2) // MMB
            {
                SnapTransform();
            }
        }
        [ClutchShortcut(id_SnapTransform, KeyCode.Keypad0, ShortcutModifiers.None)]
        static void OnSnap_Key(ShortcutArguments args)
        {
            if (!IsDisplayed()) return;
            if (args.stage == ShortcutStage.Begin)
            {
                if (EditorApplication.timeSinceStartup - tStamp_KeyDown < keyTapTime) SnapTransform(); // DoubleTap
                tStamp_KeyDown = EditorApplication.timeSinceStartup;
                FlipToggle(ref snap, tgl_Snap); HandleSnap();
            }
            else if (EditorApplication.timeSinceStartup - tStamp_KeyDown > keyTapTime) // Key was held down
            {
                FlipToggle(ref snap, tgl_Snap); HandleSnap();
            }
        }
        [Shortcut(id_ResetTransform)] static void OnSnapA() { if (!IsDisplayed()) return; ResetTransform(true); }



        static void OnSnapPos()
        {
            var button = Event.current.button;
            if (button == 0) //LMB
            {
                FlipToggle(ref snapPos, tgl_SnapPos); HandleSnap();
            }
            else if (button == 1) // RMB
            {
                ResetPos(true, true, true);
            }
            else if (button == 2) // MMB
            {
                SnapPos(snapPosX, snapPosY, snapPosZ);
            }
        }
        [ClutchShortcut(id_SnapPosition, KeyCode.Keypad1, ShortcutModifiers.None)]
        static void OnSnapPos_Key(ShortcutArguments args)
        {
            if (!IsDisplayed()) return;
            if (args.stage == ShortcutStage.Begin)
            {
                if (EditorApplication.timeSinceStartup - tStamp_KeyDown < keyTapTime) SnapPos(snapPosX, snapPosY, snapPosZ); // DoubleTap
                tStamp_KeyDown = EditorApplication.timeSinceStartup;
                FlipToggle(ref snapPos, tgl_SnapPos); HandleSnap();
            }
            else if (EditorApplication.timeSinceStartup - tStamp_KeyDown > keyTapTime) // Key was held down
            {
                FlipToggle(ref snapPos, tgl_SnapPos); HandleSnap();
            }
        }
        [Shortcut(id_ResetPosition)] static void OnSnapPosA() { if (!IsDisplayed()) return; ResetPos(true, true, true); }



        static void OnSnapPosX()
        {
            var button = Event.current.button;
            if (button == 0) //LMB
            {
                FlipToggle(ref snapPosX, tgl_SnapPosX); HandleSnap();
            }
            else if (button == 1) // RMB
            {
                ResetPos(true, false, false);
            }
            else if (button == 2) // MMB
            {
                SnapPos(true, false, false);
            }
        }



        static void OnSnapPosY()
        {
            var button = Event.current.button;
            if (button == 0) //LMB
            {
                FlipToggle(ref snapPosY, tgl_SnapPosY); HandleSnap();
            }
            else if (button == 1) // RMB
            {
                ResetPos(false, true, false);
            }
            else if (button == 2) // MMB
            {
                SnapPos(false, true, false);
            }
        }



        static void OnSnapPosZ()
        {
            var button = Event.current.button;
            if (button == 0) //LMB
            {
                FlipToggle(ref snapPosZ, tgl_SnapPosZ); HandleSnap();
            }
            else if (button == 1) // RMB
            {
                ResetPos(false, false, true);
            }
            else if (button == 2) // MMB
            {
                SnapPos(false, false, true);
            }
        }



        static void OnSnapRot()
        {
            var button = Event.current.button;
            if (button == 0) //LMB
            {
                FlipToggle(ref snapRot, tgl_SnapRot); HandleSnap();
            }
            else if (button == 1) // RMB
            {
                ResetRot(true, true, true);
            }
            else if (button == 2) // MMB
            {
                SnapRot(snapPosX, snapPosY, snapPosZ, rotationAmount);
            }
        }
        [ClutchShortcut(id_SnapRotation, KeyCode.Keypad3, ShortcutModifiers.None)]
        static void OnSnapRot_Key(ShortcutArguments args)
        {
            if (!IsDisplayed()) return;
            if (args.stage == ShortcutStage.Begin)
            {
                if (EditorApplication.timeSinceStartup - tStamp_KeyDown < keyTapTime) SnapRot(snapPosX, snapPosY, snapPosZ, rotationAmount); // DoubleTap
                tStamp_KeyDown = EditorApplication.timeSinceStartup;
                FlipToggle(ref snapRot, tgl_SnapRot); HandleSnap();
            }
            else if (EditorApplication.timeSinceStartup - tStamp_KeyDown > keyTapTime) // Key was held down
            {
                FlipToggle(ref snapRot, tgl_SnapRot); HandleSnap();
            }
        }
        [Shortcut(id_ResetRotation)] static void OnSnapRotA() { if (!IsDisplayed()) return; ResetRot(true, true, true); }



        static void OnSnapRotX()
        {
            var button = Event.current.button;
            if (button == 0) //LMB
            {
                FlipToggle(ref snapRotX, tgl_SnapRotX); HandleSnap();
            }
            else if (button == 1) // RMB
            {
                ResetRot(true, false, false);
            }
            else if (button == 2) // MMB
            {
                SnapRot(true, false, false, rotationAmount);
            }
        }



        static void OnSnapRotY()
        {
            var button = Event.current.button;
            if (button == 0) //LMB
            {
                FlipToggle(ref snapRotY, tgl_SnapRotY); HandleSnap();
            }
            else if (button == 1) // RMB
            {
                ResetRot(false, true, false);
            }
            else if (button == 2) // MMB
            {
                SnapRot(false, true, false, rotationAmount);
            }
        }



        static void OnSnapRotZ()
        {
            var button = Event.current.button;
            if (button == 0) //LMB
            {
                FlipToggle(ref snapRotZ, tgl_SnapRotZ); HandleSnap();
            }
            else if (button == 1) // RMB
            {
                ResetRot(false, false, true);
            }
            else if (button == 2) // MMB
            {
                SnapRot(false, false, true, rotationAmount);
            }
        }



        static void OnSnapScl()
        {
            var e = Event.current;
            var button = e.button;
            var modifiers = e.modifiers;
            bool shift = modifiers == EventModifiers.Shift;
            bool control = modifiers == EventModifiers.Control;
            bool none = !shift && !control;

            if (button == 0) //LMB
            {
                if (none) FlipToggle(ref snapScl, tgl_SnapScl); HandleSnap();
                if (shift) FlipScale(true, true, true); // Flip
                if (control) MultiplyScale(2, 2, 2); // Double
            }
            else if (button == 1) // RMB
            {
                if (none) ResetScl(true, true, true);
                if (control) MultiplyScale(0.5f, 0.5f, 0.5f); // Half
            }
            else if (button == 2) // MMB
            {
                SnapScl(snapPosX, snapPosY, snapPosZ);
            }
        }
        [ClutchShortcut(id_SnapScale)]
        static void OnSnapScl_Key(ShortcutArguments args)
        {
            if (!IsDisplayed()) return;
            if (args.stage == ShortcutStage.Begin)
            {
                if (EditorApplication.timeSinceStartup - tStamp_KeyDown < keyTapTime) SnapScl(snapPosX, snapPosY, snapPosZ); // DoubleTap
                tStamp_KeyDown = EditorApplication.timeSinceStartup;
                FlipToggle(ref snapScl, tgl_SnapScl); HandleSnap();
            }
            else if (EditorApplication.timeSinceStartup - tStamp_KeyDown > keyTapTime) // Key was held down
            {
                FlipToggle(ref snapScl, tgl_SnapScl); HandleSnap();
            }
        }
        [Shortcut(id_ResetScale)] static void OnSnapSclA() { if (!IsDisplayed()) return; ResetScl(true, true, true); }



        static void OnSnapSclX()
        {
            var e = Event.current;
            var button = e.button;
            var modifiers = e.modifiers;
            bool shift = modifiers == EventModifiers.Shift;
            bool control = modifiers == EventModifiers.Control;
            bool none = !shift && !control;

            if (button == 0) //LMB
            {
                if (none) FlipToggle(ref snapSclX, tgl_SnapSclX); HandleSnap();
                if (shift) FlipScale(true, false, false); // Flip
                if (control) MultiplyScale(2, 1, 1); // Double
            }
            else if (button == 1) // RMB
            {
                if (none) ResetScl(true, false, false);
                if (control) MultiplyScale(0.5f, 1, 1); // Half
            }
            else if (button == 2) // MMB
            {
                SnapScl(true, false, false);
            }
        }



        static void OnSnapSclY()
        {
            var e = Event.current;
            var button = e.button;
            var modifiers = e.modifiers;
            bool shift = modifiers == EventModifiers.Shift;
            bool control = modifiers == EventModifiers.Control;
            bool none = !shift && !control;

            if (button == 0) //LMB
            {
                if (none) FlipToggle(ref snapSclY, tgl_SnapSclY); HandleSnap();
                if (shift) FlipScale(false, true, false); // Flip
                if (control) MultiplyScale(1, 2, 1); // Double
            }
            else if (button == 1) // RMB
            {
                if (none) ResetScl(false, true, false);
                if (control) MultiplyScale(1, 0.5f, 1); // Half
            }
            else if (button == 2) // MMB
            {
                SnapScl(false, true, false);
            }
        }



        static void OnSnapSclZ()
        {
            var e = Event.current;
            var button = e.button;
            var modifiers = e.modifiers;
            bool shift   = modifiers == EventModifiers.Shift;
            bool control = modifiers == EventModifiers.Control;
            bool none = !shift && !control;

            if (button == 0) //LMB
            {
                if (none) FlipToggle(ref snapSclZ, tgl_SnapSclZ); HandleSnap();
                if (shift) FlipScale(false, false, true); // Flip
                if (control) MultiplyScale(1, 1, 2); // Double
            }
            else if (button == 1) // RMB
            {
                if (none) ResetScl(false, false, true);
                if (control) MultiplyScale(1, 1, 0.5f); // Half
            }
            else if (button == 2) // MMB
            {
                SnapScl(false, false, true);
            }
        }
        #endregion



        /////////////////////////////////////////////////////////////////////////////////////
        // HIERARCHY TOOLS ==================================================================
        /////////////////////////////////////////////////////////////////////////////////////
        #region Hierarchy Tools
        static void OnParent()
        {
            var button = Event.current.button;
            if      (button == 0) CreateParentAtOrigin(); //LMB
            else if (button == 1) CreateParentAtCenter(); //RMB
            else if (button == 2) MoveParentToCenter(Selection.activeTransform.parent); //MMB
        }
        [Shortcut(id_CreateParentAtOrigin)] static void OnParentA() { if (!IsDisplayed()) return; CreateParentAtOrigin(); }
        [Shortcut(id_CreateParentAtSelectionCenter)] static void OnParentB() { if (!IsDisplayed()) return; CreateParentAtCenter(); }
        [Shortcut(id_MoveParentToSelectionCenter)] static void OnParentC() { if (!IsDisplayed()) return; MoveParentToCenter(Selection.activeTransform.parent); }



        static void OnUnparent()
        {
            var button = Event.current.button;
            if      (button == 0) UnparentSelection();
            else if (button == 1) UnparentSelection(true);
        }
        [Shortcut(id_UnparentSelection)] static void OnUnparentA() { if (!IsDisplayed()) return; UnparentSelection(); }
        [Shortcut(id_UnparentSelectionUpInHierarchy)] static void OnUnparentB() { if (!IsDisplayed()) return; UnparentSelection(true); }
    



        [Shortcut(id_SelectParents, KeyCode.KeypadPlus, ShortcutModifiers.None)]
        static void OnHierarchyUp()
        {
            if (!IsDisplayed()) return;
            SelectParent();
        }
        [Shortcut(id_SelectChildren, KeyCode.KeypadMinus, ShortcutModifiers.None)]
        static void OnHierarchyDown()
        {
            if (!IsDisplayed()) return;
            SelectChildren();
        }



        [Shortcut(id_ChildCompensation)]
        static void OnCComp()
        {
            if (!IsDisplayed()) return;
            FlipToggle(ref cComp, tgl_CComp);
            OnChildCompensationToggled();
        }
        #endregion



        /////////////////////////////////////////////////////////////////////////////////////
        // VIEW AND SELECTION TOOLS =========================================================
        /////////////////////////////////////////////////////////////////////////////////////
        #region View and Selection Tools
        [Shortcut(id_PreviousSelection)]
        static void OnSelPrev()
        {
            if (!IsDisplayed()) return;
            LoadSelectionFromHistory(-1);
        }

        [Shortcut(id_NextSelection)]
        static void OnSelNext()
        {
            if (!IsDisplayed()) return;
            LoadSelectionFromHistory(1);
        }

        [Shortcut(id_LoadViewAndSelection1, KeyCode.Alpha1, ShortcutModifiers.Shift)]
        static void OnPreset1()
        {
            if (!IsDisplayed()) return;
            OnPreset(0);
        }
        [Shortcut(id_LoadViewAndSelection2, KeyCode.Alpha2, ShortcutModifiers.Shift)]
        static void OnPreset2()
        {
            if (!IsDisplayed()) return;
            OnPreset(1);
        }
        [Shortcut(id_LoadViewAndSelection3, KeyCode.Alpha3, ShortcutModifiers.Shift)]
        static void OnPreset3()
        {
            if (!IsDisplayed()) return;
            OnPreset(2);
        }
        [Shortcut(id_LoadViewAndSelection4, KeyCode.Alpha4, ShortcutModifiers.Shift)]
        static void OnPreset4()
        {
            if (!IsDisplayed()) return;
            OnPreset(3);
        }
        [Shortcut(id_LoadViewAndSelection5, KeyCode.Alpha5, ShortcutModifiers.Shift)]
        static void OnPreset5()
        {
            if (!IsDisplayed()) return;
            OnPreset(4);
        }
        [Shortcut(id_LoadViewAndSelection6, KeyCode.Alpha6, ShortcutModifiers.Shift)]
        static void OnPreset6()
        {
            if (!IsDisplayed()) return;
            OnPreset(5);
        }
        [Shortcut(id_LoadViewAndSelection7, KeyCode.Alpha7, ShortcutModifiers.Shift)]
        static void OnPreset7()
        {
            if (!IsDisplayed()) return;
            OnPreset(6);
        }
        [Shortcut(id_LoadViewAndSelection8, KeyCode.Alpha8, ShortcutModifiers.Shift)]
        static void OnPreset8()
        {
            if (!IsDisplayed()) return;
            OnPreset(7);
        }
        [Shortcut(id_LoadViewAndSelection9, KeyCode.Alpha9, ShortcutModifiers.Shift)]
        static void OnPreset9()
        {
            if (!IsDisplayed()) return;
            OnPreset(8);
        }
        [Shortcut(id_LoadViewAndSelection10)]
        static void OnPreset10()
        {
            if (!IsDisplayed()) return;
            OnPreset(9);
        }
        [Shortcut(id_LoadViewAndSelection11)]
        static void OnPreset11()
        {
            if (!IsDisplayed()) return;
            OnPreset(10);
        }
        [Shortcut(id_LoadViewAndSelection12)]
        static void OnPreset12()
        {
            if (!IsDisplayed()) return;
            OnPreset(11);
        }
        #endregion
    }
}
