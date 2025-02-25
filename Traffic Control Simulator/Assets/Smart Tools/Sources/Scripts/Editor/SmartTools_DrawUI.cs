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
    public partial class SmartTools
    {
        static public bool fullyInitialized;

        public override VisualElement CreatePanelContent()
        {
            if (instance == null) instance = this;
            Initialize();
            CreatePanel();
            root.schedule.Execute(() => { fullyInitialized = true; }).StartingIn(2);
            //UpdateAllPresetButtons();  
            return root;
        }


        public override void OnWillBeDestroyed()
        {
            instance = null;
            fullyInitialized = false;
            Deinitialize();
        }


        static void CreatePanel()
        {
            tgl_GridVisiblity = GetButton(r[4], On_btn_ShowGrid, "Grid", tt_Grid, c.textBright, c.buttonBright);
            tgl_Settings = GetButton(r[3], On_btn_ShowSettings, uc_Settings, tt_Settings, c.textBright, c.buttonBright);
            NewLine(newline);

            sld_ViewDependency = GetSlider(On_Sld_ViewDependency, tt_ViewDependency, 0, 1, 0.5f);
            NewLine(newline);

            sld_MoveAreaSize = GetSlider(On_Sld_MoveAreaSize, tt_MoveAreaSize, 0, 1, 0);
            NewLine(newline);

            btn_GridSize = GetButton(r[1], On_btn_GridSize, uc_Grid, tt_GridSize, c.textBright, c.buttonBright);
            flf_GridSize = GetFloatField(r[5], On_flf_GridSize);
            NewLine(newline);

            lbl_Increment = GetLabel(r[0], "Increment", tt_Increments, c.textDefault);
            NewLine(newline);

            tgl_IncrementA = GetButton(r[1], On_btn_IncrementA, "A", tt_IncSwitch, c.textPos, c.buttonPos);
            flf_IncrementA = GetFloatField(r[5], On_flf_IncrementA);
            NewLine(newline);

            tgl_IncrementB = GetButton(r[1], On_btn_IncrementB, "B", tt_IncSwitch, c.textPos, c.buttonPos);
            flf_IncrementB = GetFloatField(r[5], On_flf_IncrementB);
            NewLine(newline);

            lbl_Angle = GetLabel(r[0], "Angle", tt_Degrees, c.textDefault);
            tgl_Pivot = GetButton(r[3], On_btn_Pivot, uc_Pivot, tt_Pivot, c.textRot, c.buttonRot); tgl_Pivot.highlight = true;
            NewLine(newline);

            tgl_AngleA = GetButton(r[1], On_btn_AngleA, "A", tt_DegSwitch, c.textRot, c.buttonRot);
            flf_AngleA = GetFloatField(r[5], On_flf_AngleA);
            NewLine(newline);

            tgl_AngleB = GetButton(r[1], On_btn_AngleB, "B", tt_DegSwitch, c.textRot, c.buttonRot);
            flf_AngleB = GetFloatField(r[5], On_flf_AngleB);
            NewLine(newline);

            tgl_RotAxisX = GetButton(r[1], On_btn_AxisX, "X", tt_RotAxisX, c.textX, c.buttonRot);
            tgl_RotAxisY = GetButton(r[2], On_btn_AxisY, "Y", tt_RotAxisY, c.textY, c.buttonRot);
            tgl_RotAxisZ = GetButton(r[3], On_btn_AxisZ, "Z", tt_RotAxisZ, c.textZ, c.buttonRot);
            NewLine(newline);

            NewLine(spacer);

            btn_RotLeft = GetButton(r[1], OnRotateLeft, uc_ArrowL, tt_RotateL, c.textRot, c.buttonRot);
            btn_MoveForward = GetButton(r[2], OnMoveUp, uc_ArrowU, tt_MoveUp, c.textPos, c.buttonPos);
            btn_RotRight = GetButton(r[3], OnRotateRight, uc_ArrowR, tt_RotateR, c.textRot, c.buttonRot);
            NewLine(newline);
            btn_MoveLeft = GetButton(r[1], OnMoveLeft, uc_ArrowL, tt_MoveL, c.textPos, c.buttonPos);
            btn_MoveBack = GetButton(r[2], OnMoveDown, uc_ArrowD, tt_MoveD, c.textPos, c.buttonPos);
            btn_MoveRight = GetButton(r[3], OnMoveRight, uc_ArrowR, tt_MoveR, c.textPos, c.buttonPos);
            NewLine(newline);
            btn_MoveUp = GetButton(r[1], OnMoveForward, uc_ArrowU, tt_MoveF, c.textPos, c.buttonPos);
            btn_MoveDown = GetButton(r[2], OnMoveBack, uc_ArrowD, tt_MoveB, c.textPos, c.buttonPos);
            btn_Duplicate = GetButton(r[3], OnDuplicate, "D", tt_Duplicate, c.textDefault, c.buttonDefault);
            NewLine(newline);

            tgl_Focus = GetButton(r[0], OnFocus, "Focus", tt_Focus, c.textDefault, c.buttonDefault); tgl_Focus.highlight = true;
            NewLine(newline);

            sld_HorizontalMode = GetSlider(On_Sld_HorizontalMode, tt_HorizontalMode, -1, 1, 0.0f);
            NewLine(newline);

            tgl_Snap = GetButton(r[0], OnSnap, "Snap", tt_Snap, c.textBright, c.buttonBright);
            NewLine(newline);
            tgl_SnapPos = GetButton(r[0], OnSnapPos, "Position", tt_SnapPos, c.textPos, c.buttonPos);
            NewLine(newline);
            tgl_SnapPosX = GetButton(r[1], OnSnapPosX, "X", tt_SnapPosX, c.textX, c.buttonPos);
            tgl_SnapPosY = GetButton(r[2], OnSnapPosY, "Y", tt_SnapPosY, c.textY, c.buttonPos);
            tgl_SnapPosZ = GetButton(r[3], OnSnapPosZ, "Z", tt_SnapPosZ, c.textZ, c.buttonPos);
            NewLine(newline);

            tgl_SnapRot = GetButton(r[0], OnSnapRot, "Rotation", tt_SnapRot, c.textRot, c.buttonRot);
            NewLine(newline);
            tgl_SnapRotX = GetButton(r[1], OnSnapRotX, "X", tt_SnapRotX, c.textX, c.buttonRot);
            tgl_SnapRotY = GetButton(r[2], OnSnapRotY, "Y", tt_SnapRotY, c.textY, c.buttonRot);
            tgl_SnapRotZ = GetButton(r[3], OnSnapRotZ, "Z", tt_SnapRotZ, c.textZ, c.buttonRot);
            NewLine(newline);

            tgl_SnapScl = GetButton(r[0], OnSnapScl, "Scale", tt_SnapScl, c.textScl, c.buttonScl);
            NewLine(newline);
            tgl_SnapSclX = GetButton(r[1], OnSnapSclX, "X", tt_SnapSclX, c.textX, c.buttonScl);
            tgl_SnapSclY = GetButton(r[2], OnSnapSclY, "Y", tt_SnapSclY, c.textY, c.buttonScl);
            tgl_SnapSclZ = GetButton(r[3], OnSnapSclZ, "Z", tt_SnapSclZ, c.textZ, c.buttonScl);
            NewLine(newline);

            NewLine(spacer);

            btn_Parent = GetButton(r[0], OnParent, "Parent", tt_Parent, c.textDefault, c.buttonDefault);
            NewLine(newline);
            btn_HierarchyUp = GetButton(r[1], OnHierarchyUp, uc_ArrowU, tt_SelParents, c.textDefault, c.buttonDefault);
            btn_HierarchyDown = GetButton(r[2], OnHierarchyDown, uc_ArrowD, tt_SelChildren, c.textDefault, c.buttonDefault);
            btn_Unparent = GetButton(r[3], OnUnparent, "U", tt_Unparent, c.textDefault, c.buttonDefault);
            NewLine(newline);
            tgl_CComp = GetButton(r[0], OnCComp, "C.Comp.", tt_CComp, c.textDefault, c.buttonCComp); tgl_CComp.highlight = true;
            NewLine(newline);

            NewLine(spacer);

            btn_SelPrev = GetButton(r[6], OnSelPrev, uc_ArrowL, tt_PrevSel, c.textDefault, c.buttonDark);
            btn_SelNext = GetButton(r[7], OnSelNext, uc_ArrowR, tt_NextSel, c.textDefault, c.buttonDark);
            NewLine(newline);

            btn_Preset = new MyButton[12];
            for (int i = 0; i < btn_Preset.Length; i++)
            {
                int rectIndex = (i % 3) + 1;
                btn_Preset[i] = GetButton(r[rectIndex], presetActions[i], GetString(sceneData.store[i].cam.isSaved, sceneData.store[i].cam.isSaved), PresetButtonTooltip(i), c.textDefault, c.buttonDark);
                btn_Preset[i].index = i;
                btn_Preset[i].style.fontSize = 11;
                if (rectIndex == 3) NewLine(newline); 
            }


            // Size the arrows
            btn_RotLeft.style.fontSize = 10;
            btn_MoveForward.style.fontSize = 8;
            btn_RotRight.style.fontSize = 8;
            btn_MoveLeft.style.fontSize = 10;
            btn_MoveBack.style.fontSize = 8;
            btn_MoveRight.style.fontSize = 8;
            btn_MoveUp.style.fontSize = 8;
            btn_MoveDown.style.fontSize = 8;
            btn_HierarchyUp.style.fontSize = 8;
            btn_HierarchyDown.style.fontSize = 8;
            btn_SelPrev.style.fontSize = 10;
            btn_SelNext.style.fontSize = 8;

            // Setup the startup values
            tgl_GridVisiblity.UpdateToggle(showGrid);
            tgl_Settings.UpdateToggle(showSettings);
            sld_ViewDependency.value = viewDependency = 0.5f;
            sld_MoveAreaSize.value = moveAreaSize = 0;
            flf_GridSize.value = gridSize = 1;
            flf_IncrementA.value = moveIncrementA = 1;
            flf_IncrementB.value = moveIncrementB = 4;
            flf_AngleA.value = rotIncrementA = 22.5f;
            flf_AngleB.value = rotIncrementB = 90.0f;

            tgl_IncrementA.UpdateToggle(moveIncrement);
            tgl_IncrementB.UpdateToggle(!moveIncrement);
            tgl_Pivot.UpdateToggle(pivot);
            tgl_AngleA.UpdateToggle(rotIncrement);
            tgl_AngleB.UpdateToggle(!rotIncrement);
            rotAxis = 1; UpdateRotAxisButtons();

            sld_HorizontalMode.value = horizontalMode = 0;

            tgl_Snap.UpdateToggle(snap);
            tgl_SnapPos.UpdateToggle(snapPos);
            tgl_SnapPosX.UpdateToggle(snapPosX);
            tgl_SnapPosY.UpdateToggle(snapPosY);
            tgl_SnapPosZ.UpdateToggle(snapPosZ);
            tgl_SnapRot.UpdateToggle(snapRot);
            tgl_SnapRotX.UpdateToggle(snapRotX);
            tgl_SnapRotY.UpdateToggle(snapRotY);
            tgl_SnapRotZ.UpdateToggle(snapRotZ);
            tgl_SnapScl.UpdateToggle(snapScl);
            tgl_SnapSclX.UpdateToggle(snapSclX);
            tgl_SnapSclY.UpdateToggle(snapSclY);
            tgl_SnapSclZ.UpdateToggle(snapSclZ);

            tgl_CComp.UpdateToggle(cComp);

            UpdateMoveDistance();
            UpdateRotAmount();

            root.style.height = r[0].y;

            CreateHelpArea();
            root.style.overflow = Overflow.Hidden;
        }

        const float helpAreaWidth = 648;

        static void CreateHelpArea()
        {
            float x = 64;
            float w = 64 + helpAreaWidth - (x + 12) - padding;

            Rect r_Arrow = new Rect(x + 4, padding, h, h);
            Rect r_Frame = new Rect(x + 12, padding, w, h);
            Rect r_Label = new Rect(x + 24, padding, w, h);

            F();
            L("Shows/hides the grid. The button next to it opens this help screen."); N(newline);
            F();
            L("Changes the view dependency of the grid."); N(newline);
            F();
            L("Allows you to add opacity to the grid. This can help with relative positioning."); N(newline);
            F();
            L("Cycles through some grid size presets."); N(newline);

            N(newline);

            F(2);
            L("Changes the main & secondary move increment (dependent on the grid size)."); N(newline);
            L("The buttons toggle between these (middle/right-click to cycle through some preset values)."); N(newline);

            F();
            L("Toggles between default and pivot-related rotation mode (only affects multi-selections)."); N(newline);

            F(2);
            L("Changes the main & secondary rotation angles(these values are also affecting the rotation snapping)."); N(newline);
            L("The buttons toggle between these (middle/right-click to cycle through some preset values)."); N(newline);

            F();
            L("Toggles for selecting the current rotation axis (for rotation buttons/hotkeys)."); N(newline);

            N(spacer);

            F(3);
            L("Move/rotate-buttons for quick and convenient placement."); N(newline);
            L("Left-clicking will use the selected increment/angle, right-clicking uses the other increment/angle."); N(newline);
            L("The \"D\" button is for quickly duplicating the selected object(s)."); N(newline);

            F();
            L("When Focus is enabled, it will always keep the selection in the center of the screen."); N(newline);
            F();
            L("Switches between free and horizontal mode (see manual for more details - default is all the way to the left)."); N(newline);

            F(7, true);
            L("Left-click toggles all snapping, middle-click snaps, right-click resets the transforms of the selected object(s)."); N(newline);
            L("The same for position snapping."); N(newline);
            L("The same but per axis."); N(newline);
            L("The same for rotation snapping."); N(newline);
            L("The same but per axis."); N(newline);
            L("The same for scale snapping. For additional functionlaity check the tooltips."); N(newline);
            L("The same but per axis. For additional functionlaity check the tooltips."); N(newline);

            N(spacer);

            F();
            L("Quick-parenting (see tooltip/manual for more details)."); N(newline);
            F();
            L("HierarchyUp / HierarchyDown / Unparent (see tooltips/manual for more information)."); N(newline);
            F();
            L("Child-Compensation - When enabled, the transform of the parent won’t affect the child(ren)."); N(newline);

            N(spacer);

            F();
            L("Cycles through your previous selections (takes the project window into account)."); N(newline);

            F(4);
            L("Here you can save/load selections and camera views."); N(newline);
            L("Selections and views can be stored together or separately."); N(newline);
            L("Check the tooltips for more details."); N(newline);
            L(""); N(newline);

            void L(string text)
            {
                GetLabel(r_Label, text, "", c.textDefault);
            }
            void N(float amount)
            {
                r_Label.y += amount;
                r_Frame.y += amount;
                r_Arrow.y += amount;

            }
            void F(int lines = 1, bool first = false)
            {
                float height = h * lines + spacing * (lines - 1);

                Rect r = r_Arrow;
                if (!first && height > 1) r.height = height;
                GetLabel(r, uc_ArrowL, "", c.textDefault);

                var ve = new VisualElement();

                ve.style.left = r_Frame.x;
                ve.style.top = r_Frame.y;
                ve.style.width = r_Frame.width;
                ve.style.height = (height == 0) ? r_Frame.height : height;

                float radius = c.buttonRadius;

                ve.style.position = Position.Absolute;
                ve.style.marginLeft = 0;
                ve.style.marginTop = 0;
                ve.style.marginRight = 0;
                ve.style.marginBottom = 0;
                ve.style.paddingLeft = 0;
                ve.style.paddingTop = 0;
                ve.style.paddingRight = 0;
                ve.style.paddingBottom = 0;
                ve.style.borderBottomLeftRadius = radius;
                ve.style.borderTopLeftRadius = radius;
                ve.style.borderTopRightRadius = radius;
                ve.style.borderBottomRightRadius = radius;
                ve.style.borderLeftWidth = 4;
                ve.style.borderTopWidth = 0;
                ve.style.borderRightWidth = 0;
                ve.style.borderBottomWidth = 0;
                ve.style.borderLeftColor = c.textDefault;
                ve.style.backgroundColor = c.background * 1.1f;
                ve.style.opacity = 1;

                root.Add(ve);
            }

            // Offset
            N(-newline);

            float lw = 12;
            float fw = 60;

            Rect b = new Rect(r_Frame);
            b.y -= newline + padding;
            b.x += r_Frame.width;
            b.x -= lw * 3 + fw * 3 + padding * 4;
            b.width = lw * 3 + fw * 3 + padding * 4;
            b.height = newline * 2;
            F2(b);

            Rect f1 = new Rect(r_Frame);
            f1.x += f1.width - fw;
            f1.width = fw;
            Rect l1 = new Rect(r_Frame);
            l1.x = f1.x - lw;
            l1.width = lw;

            GetLabel(l1, "Z", "", c.textDefault);
            flf_GridOffsetZ = GetFloatField(f1, OnGridOffsetZ);
            f1.x -= fw + padding + lw;
            l1.x -= fw + padding + lw;
            GetLabel(l1, "Y", "", c.textDefault);
            flf_GridOffsetY = GetFloatField(f1, OnGridOffsetY);
            f1.x -= fw + padding + lw;
            l1.x -= fw + padding + lw;
            GetLabel(l1, "X", "", c.textDefault);
            flf_GridOffsetX = GetFloatField(f1, OnGridOffsetX);
            l1.y -= newline;
            GetLabel(l1, "Global Grid Offset", "", c.textDefault);

            flf_GridOffsetX.value = sceneData.gridOffset.x;
            flf_GridOffsetY.value = sceneData.gridOffset.y;
            flf_GridOffsetZ.value = sceneData.gridOffset.z;

            if (SmartTools_Settings.logo == null) settings.OnValidate();

            var texScale = 0.5f;
            var logo = new VisualElement();
            logo.style.left = padding + r_Frame.x + r_Frame.width - SmartTools_Settings.logo.width * texScale - 12;
            logo.style.top = 12;
            logo.style.width = SmartTools_Settings.logo.width * texScale;
            logo.style.height = SmartTools_Settings.logo.height * texScale;
            logo.style.backgroundImage = (Texture2D)SmartTools_Settings.logo;
            root.Add(logo);



            void F2(Rect r)
            {
                var ve = new VisualElement();

                ve.style.left = r.x;
                ve.style.top = r.y;
                ve.style.width = r.width;
                ve.style.height = r.height;

                float radius = c.buttonRadius;

                ve.style.position = Position.Absolute;
                ve.style.marginLeft = 0;
                ve.style.marginTop = 0;
                ve.style.marginRight = 0;
                ve.style.marginBottom = 0;
                ve.style.paddingLeft = 0;
                ve.style.paddingTop = 0;
                ve.style.paddingRight = 0;
                ve.style.paddingBottom = 0;
                ve.style.borderBottomLeftRadius = 0;
                ve.style.borderTopLeftRadius = radius;
                ve.style.borderTopRightRadius = 0;
                ve.style.borderBottomRightRadius = 0;
                ve.style.borderLeftWidth = 0;
                ve.style.borderTopWidth = 0;
                ve.style.borderRightWidth = 0;
                ve.style.borderBottomWidth = 0;
                ve.style.backgroundColor = c.background;
                ve.style.opacity = 1;

                root.Add(ve);
            }
        }


        static void OnGridOffsetX(ChangeEvent<float> evt)
        {
            sceneData.gridOffset = new Vector3(evt.newValue, sceneData.gridOffset.y, sceneData.gridOffset.z);
            EditorUtility.SetDirty(sceneData);
        }
        static void OnGridOffsetY(ChangeEvent<float> evt)
        {
            sceneData.gridOffset = new Vector3(sceneData.gridOffset.x, evt.newValue, sceneData.gridOffset.z);
            EditorUtility.SetDirty(sceneData);
        }
        static void OnGridOffsetZ(ChangeEvent<float> evt)
        {
            sceneData.gridOffset = new Vector3(sceneData.gridOffset.x, sceneData.gridOffset.y, evt.newValue);
            EditorUtility.SetDirty(sceneData);
        }




        static void HandleFloatPresetButtons(float[] presets, FloatField flf, ref float storedValue, int buttonA, int buttonB)
        {
            var button = Event.current.button;

            float value = flf.value;
            int next = -1;
            for (int i = 0; i < presets.Length; i++)
            {
                float presetVal = presets[i];
                if (value <= presetVal)
                {
                    next = i;
                    break;
                }
            }
            if (button == buttonA)
            {
                next++;
                if (value >= presets[presets.Length - 1]) next = 0;
                flf.value = presets[next];
            }
            else if (button == buttonB)
            {
                next--;
                if (next < 0) next = presets.Length - 1;
                flf.value = presets[next];
            }
            storedValue = flf_GridSize.value;

            // Aligns the overflow when the button  is pressed
            flf.schedule.Execute(() =>
            {
                var textElement = flf.hierarchy.ElementAt(0).hierarchy.ElementAt(0);
                textElement.style.translate = new Translate(0, 0, 0);
                textElement.MarkDirtyRepaint();
            }).StartingIn(2);
        }



        static void HandleFloatFields(FloatField flf, ref float storedValue, float min = 0.001f, float max = float.PositiveInfinity)
        {
            var value = flf.value;
            if      (value < min) value = min;
            else if (value > max) value = max;
            flf.value = storedValue = value;
        }
    }
}
