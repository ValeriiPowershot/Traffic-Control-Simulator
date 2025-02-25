using System;
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
        static public void RefreshOverlay()
        {
            if (instance == null || !fullyInitialized) return;
            instance.displayed = false;
            instance.displayed = true;
        }

        static void SetupLayout()
        {
            w = 16;
            h = 16;
            padding = 4;
            spacing = 4;
            spacer = 8;
            newline = h + spacing;

            float w1 = w;
            float w2 = w * 2 + spacing;
            float w3 = w * 3 + spacing * 2;
            float w4 = (w * 3 + spacing * 2) / 2 - spacing / 2;

            float x1 = padding;
            float x2 = x1 + spacing + w1;
            float x3 = x1 + spacing + w1 + spacing + w1;
            float x4 = x1 + w4 + spacing;
            float baseline = padding;


            r = new Rect[8];
            r[0] = new Rect(x1, baseline, w3, h);
            r[1] = new Rect(x1, baseline, w1, h);
            r[2] = new Rect(x2, baseline, w1, h);
            r[3] = new Rect(x3, baseline, w1, h);
            r[4] = new Rect(x1, baseline, w2, h);
            r[5] = new Rect(x2, baseline, w2, h);
            r[6] = new Rect(x1, baseline, w4, h);
            r[7] = new Rect(x4, baseline, w4, h);
        }


        static void StyleVE(VisualElement ve, float x, float y, float w, float h)
        {
            ve.style.left = x;
            ve.style.top = y;
            ve.style.width = w;
            ve.style.height = h;
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
            ve.style.borderLeftWidth = 0;
            ve.style.borderTopWidth = 0;
            ve.style.borderRightWidth = 0;
            ve.style.borderBottomWidth = 0;
        }

        static void SetupRoot()
        {
            root = new VisualElement() { name = "Smart Tools Root Visual Element" };
            StyleVE(root, 0, 0, 64 + (showSettings ? 724 : 0) , 648);
            root.style.backgroundColor = c.background;
            root.style.position = Position.Relative;
            root.tooltip = "";
            root.schedule.Execute(() => {
                float margin = 2;
                float padding = 0;
                float radiusTop = 6;
                float radiusBottom = 6;
                var p = root.parent.parent;
                p.style.backgroundColor = new Color(0, 0, 0, 0.125f);
                p.style.marginLeft = margin;
                p.style.marginTop = margin;
                p.style.marginRight = margin;
                p.style.marginBottom = margin;
                p.style.paddingLeft = padding;
                p.style.paddingTop = padding;
                p.style.paddingRight = padding;
                p.style.paddingBottom = padding;
                p.style.borderTopLeftRadius = radiusTop;
                p.style.borderTopRightRadius = radiusTop;
                p.style.borderBottomLeftRadius = radiusBottom;
                p.style.borderBottomRightRadius = radiusBottom;
                p.style.borderLeftWidth = 0;
                p.style.borderTopWidth = 0;
                p.style.borderRightWidth = 0;
                p.style.borderBottomWidth = 0;
            });
        }



        static void NewLine(float amount)
        {
            for (int i = 0; i < r.Length; i++) r[i].y += amount;
        }



        static MyButton GetButton(Rect r, Action OnClickCallback, string text, string tooltip, Color cFG, Color cBG)
        {
            var btn = new MyButton();
            StyleVE(btn, r.x, r.y, r.width, r.height);

            btn.enableRichText = true;
            btn.text = text;
            btn.tooltip = tooltip;
            btn.cFG = cFG;
            btn.cBG = cBG;
            btn.style.color = btn.cFG;
            btn.style.backgroundColor = btn.cBG;
            btn.style.unityTextAlign = TextAnchor.MiddleCenter;

            btn.RegisterCallback<MouseDownEvent>(evt => OnClickCallback());
            btn.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            btn.RegisterCallback<MouseLeaveEvent>(OnMouseExit);

            root.Add(btn);
            return btn;
        }




        static Slider GetSlider(EventCallback<ChangeEvent<float>> OnSliderChangeCallback, string tooltip, float min, float max, float defaultValue = 0)
        {
            var sld = new Slider(min, max);
            sld.tooltip = tooltip;

            StyleVE(sld, r[0].x, r[0].y - 2, r[0].width, r[0].height);

            sld.value = 0.66f;
            sld.focusable = false;
            sld.RegisterValueChangedCallback(OnSliderChangeCallback);

            // Register a MouseDownEvent to detect right-clicks
            sld.RegisterCallback<MouseDownEvent>(evt =>
            {
                if (evt.button == 1) 
                {
                    sld.value = defaultValue;
                }
            });

            root.Add(sld);
            return sld;
        }



        static Label GetLabel(Rect r, string text, string tooltip, Color cFG)
        {
            var lbl = new Label();
            StyleVE(lbl, r.x - 1, r.y, r.width, r.height);

            lbl.style.color = cFG;
            lbl.text = text;
            lbl.tooltip = tooltip;
            lbl.style.unityTextAlign = TextAnchor.MiddleLeft;
            lbl.focusable = false;

            root.Add(lbl);
            return lbl;
        }



        static FloatField GetFloatField(Rect r, EventCallback<ChangeEvent<float>> OnFloatFieldChangeEvent)
        {
            var flf = new FloatField();
            StyleVE(flf, r.x, r.y, r.width, r.height);

            flf.hierarchy.ElementAt(0);

            var child1 = flf.hierarchy.ElementAt(0); //flf.Q("unity-text-input");
            var child2 = child1.hierarchy.ElementAt(0); //child1.Q(".unity-text-element");
            child1.style.paddingLeft = 0;
            child1.style.marginLeft = 0;
            child1.style.paddingRight = 0;
            child1.style.marginRight = 0;
            child2.style.paddingLeft = 0;
            child2.style.marginLeft = 0;
            child2.style.paddingRight = 0;
            child2.style.marginRight = 0;

            flf.style.unityTextAlign = TextAnchor.MiddleLeft;
            //flf.focusable = false;
            flf.RegisterValueChangedCallback(OnFloatFieldChangeEvent);

            root.Add(flf);
            return flf;
        }

        static void OnMouseEnter(MouseEnterEvent evt)
        {
            MyButton b = (MyButton)evt.target;
            if (!b.enabledSelf) return;
            if (b.highlight && b.style.color == c.textHighlight)
            {
                b.style.color = c.textHighlight * c.hoverMulti;
                b.style.backgroundColor = c.buttonHighlight * c.hoverMulti;
            }
            else
            {
                b.style.color = b.cFG * c.hoverMulti;
                b.style.backgroundColor = b.cBG * c.hoverMulti;
            }
        }

        static void OnMouseExit(MouseLeaveEvent evt)
        {
            MyButton b = (MyButton)evt.target;
            if (!b.enabledSelf) return;
            if (b.highlight && (b.style.color == c.textHighlight || b.style.color == c.textHighlight * c.hoverMulti))
            {
                b.style.color = c.textHighlight;
                b.style.backgroundColor = c.buttonHighlight;
            }
            else
            {
                b.style.color = b.cFG;
                b.style.backgroundColor = b.cBG;
            }
        }

        static void FlipToggle(ref bool b, MyButton tgl)
        {
            b = !b;
            tgl.UpdateToggle(b);
        }
        static void Toggle2Way(ref bool b, MyButton tglA, MyButton tglB)
        {
            b = !b;
            tglA.UpdateToggle(b);
            tglB.UpdateToggle(!b);
        }

        static void UpdateRotAxisButtons()
        {
            tgl_RotAxisX.UpdateToggle(rotAxis == 0);
            tgl_RotAxisY.UpdateToggle(rotAxis == 1);
            tgl_RotAxisZ.UpdateToggle(rotAxis == 2);
        }

        static void HandleSnap()
        {
            tgl_SnapPos.SetEnabled(snap);             tgl_SnapPos. UpdateToggle(snap && snapPos);
            tgl_SnapPosX.SetEnabled(snap && snapPos); tgl_SnapPosX.UpdateToggle(snap && snapPos && snapPosX);
            tgl_SnapPosY.SetEnabled(snap && snapPos); tgl_SnapPosY.UpdateToggle(snap && snapPos && snapPosY);
            tgl_SnapPosZ.SetEnabled(snap && snapPos); tgl_SnapPosZ.UpdateToggle(snap && snapPos && snapPosZ);
            tgl_SnapRot.SetEnabled(snap);             tgl_SnapRot. UpdateToggle(snap && snapRot);
            tgl_SnapRotX.SetEnabled(snap && snapRot); tgl_SnapRotX.UpdateToggle(snap && snapRot && snapRotX);
            tgl_SnapRotY.SetEnabled(snap && snapRot); tgl_SnapRotY.UpdateToggle(snap && snapRot && snapRotY);
            tgl_SnapRotZ.SetEnabled(snap && snapRot); tgl_SnapRotZ.UpdateToggle(snap && snapRot && snapRotZ);
            tgl_SnapScl.SetEnabled(snap);             tgl_SnapScl. UpdateToggle(snap && snapScl);
            tgl_SnapSclX.SetEnabled(snap && snapScl); tgl_SnapSclX.UpdateToggle(snap && snapScl && snapSclX);
            tgl_SnapSclY.SetEnabled(snap && snapScl); tgl_SnapSclY.UpdateToggle(snap && snapScl && snapSclY);
            tgl_SnapSclZ.SetEnabled(snap && snapScl); tgl_SnapSclZ.UpdateToggle(snap && snapScl && snapSclZ);
        }



        static Action[] presetActions = { OnPreset1, OnPreset2, OnPreset3, OnPreset4, OnPreset5, OnPreset6, OnPreset7, OnPreset8, OnPreset9, OnPreset10, OnPreset11, OnPreset12 };


        class MyButton : Label
        {
            public Color cFG;
            public Color cBG;
            public bool highlight;
            public int index = -1;

            public void UpdateToggle(bool b)
            {
                if (highlight)
                {
                    style.color = b ? c.textHighlight : cFG;
                    style.backgroundColor = b ? c.buttonHighlight : cBG;
                }
                else
                {
                    style.opacity = b ? 1 : c.disabledOpacity;
                }
            }
        }
    }
}
