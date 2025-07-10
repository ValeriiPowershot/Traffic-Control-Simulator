using System;
using UnityEditor;
using UnityEngine;

namespace Gley.UrbanAssets.Editor
{
    public abstract class SettingsWindowBase : EditorWindow
    {
        public delegate void RefreshWindow();
        public static RefreshWindow onRefreshWindow;
        public static bool firstanchorClicked;

        public static void TriggerRefreshWindowEvent()
        {
            if (onRefreshWindow != null)
            {
                onRefreshWindow();
            }
        }

        protected SettingsWindowBase window;
        protected ISetupWindow activeSetupWindow;
        protected bool initialized;

        private WindowProperties[] allWindowsData;
        private AllSettingsWindows allSettingsWindows;
        private NavigationRuntimeData backData;
        private SceneCameraTracker sceneCameraTracker;
        private Type defaultWindow;
        private RaycastHit hitInfo;
        private bool playState;
        private bool blockClicks;
        private bool canClick;

        internal abstract LayerMask GetGroundLayer();
        protected abstract void Reinitialize();
        protected abstract void MouseMove(Vector3 point);


        #region Initialization
        protected void Init(SettingsWindowBase window, Type defaultWindowType, WindowProperties[] allWindowsProperties, AllSettingsWindows allSettingsWindows)
        {
            this.allSettingsWindows = allSettingsWindows;
            initialized = false;
            this.window = window;
            allWindowsData = allWindowsProperties;
            defaultWindow = defaultWindowType;
            sceneCameraTracker = new SceneCameraTracker();
            ResetToHomeScreen(defaultWindow, false);
        }


        protected virtual void ResetToHomeScreen(Type defaultWindow, bool now)
        {
            if (defaultWindow == null || allSettingsWindows == null || allWindowsData == null)
            {
                initialized = false;
                Reinitialize();
            }
            else
            {
                if (!now)
                {
                    if (initialized == true)
                        return;
                }
                initialized = true;
                playState = Application.isPlaying;
                allSettingsWindows.Initialize(allWindowsData);
                backData = new NavigationRuntimeData(allSettingsWindows);
                SetActiveWindow(defaultWindow, false);
                SceneView.RepaintAll();
            }
        }


        protected virtual void OnEnable()
        {
            onRefreshWindow -= Refresh;
            onRefreshWindow += Refresh;
            SceneView.duringSceneGui += OnScene;
        }


        protected virtual void OnDisable()
        {
            onRefreshWindow -= Refresh;
            BlockClicks(false);
            SceneView.duringSceneGui -= OnScene;
        }


        protected virtual void OnFocus()
        {
            ResetToHomeScreen(defaultWindow, false);
        }
        #endregion


        #region WindowNavigation
        internal void SetActiveWindow(Type windowType, bool addCurrent)
        {
            if (windowType == null)
            {
                return;
            }

            if (activeSetupWindow != null)
            {
                activeSetupWindow.DestroyWindow();
            }

            if (addCurrent)
            {
                backData.AddWindow(activeSetupWindow.GetFullClassName());
            }
            activeSetupWindow = ((SetupWindowBase)CreateInstance(windowType)).Initialize(allSettingsWindows.GetWindowProperties(windowType.Name), this);
            BlockClicks(activeSetupWindow.GetBlockClicksState());
            if (window)
            {
                window.Repaint();
            }
        }


        internal string GetBackPath()
        {
            return backData.GetBackPath();
        }


        private void Back()
        {
            SetActiveWindow(Type.GetType(backData.RemoveLastWindow()), false);
        }


        private void OnDestroy()
        {
            if (activeSetupWindow != null)
            {
                activeSetupWindow.DestroyWindow();
            }
        }
        #endregion


        #region WindowGUI
        protected virtual void OnGUI()
        {
            if (playState != Application.isPlaying)
            {
                ResetToHomeScreen(defaultWindow, true);
            }
            EditorStyles.label.wordWrap = true;
            EditorGUILayout.Space();

            if (activeSetupWindow == null)
            {
                if (defaultWindow == null)
                {
                    return;
                }
                ResetToHomeScreen(defaultWindow, false);
            }

            if (activeSetupWindow.DrawInWindow(position.width, position.height) == false)
            {
                Back();
            }
        }


        void Refresh()
        {
            if (window)
            {
                window.Repaint();
            }
        }
        #endregion


        #region SceneDisplay
        protected virtual void OnScene(SceneView obj)
        {
            if (playState != Application.isPlaying)
            {
                ResetToHomeScreen(defaultWindow, true);
            }

            if (GleyPrefabUtilities.PrefabChanged())
            {
                ResetToHomeScreen(defaultWindow, true);
            }

            if (blockClicks == false)
                return;

            Color handlesColor = Handles.color;
            Matrix4x4 handlesMatrix = Handles.matrix;
            Draw();
            Input();
            sceneCameraTracker.MoveCheck();
            Handles.color = handlesColor;
            Handles.matrix = handlesMatrix;
        }


        internal void BlockClicks(bool block)
        {
            if (window)
            {
                window.blockClicks = block;
            }
        }


        private void Input()
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Event e = Event.current;
            if (e.type == EventType.KeyDown && e.control && e.keyCode == KeyCode.Z)
            {
                UndoAction();
            }

            if (e.type == EventType.MouseMove)
            {
                Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);

                if (GleyPrefabUtilities.EditingInsidePrefab())
                {
                    if (GleyPrefabUtilities.GetScenePrefabRoot().scene.GetPhysicsScene().Raycast(worldRay.origin, worldRay.direction, out hitInfo, Mathf.Infinity, GetGroundLayer()))
                    {
                        canClick = true;
                    }
                    else
                    {
                        canClick = false;
                    }
                }
                else
                {
                    if (Physics.Raycast(worldRay, out hitInfo, Mathf.Infinity, GetGroundLayer()))
                    {
                        canClick = true;
                    }
                    else
                    {
                        canClick = false;
                    }
                }
                MouseMove(hitInfo.point);
            }

            if (canClick)
            {

#if GLEY_PEDESTRIAN_SYSTEM
                if (e.type == EventType.Layout && GUIUtility.hotControl == Gley.PedestrianSystem.Internal.Constants.pathID)
                {
                    //click on the first gizmo
                    if (e.shift)
                    {
                        firstanchorClicked = true;
                        LeftClick(hitInfo.point, firstanchorClicked);
                    }
                }
                else
                {
                    firstanchorClicked = false;
                }
#endif

                if (e.type == EventType.MouseDown && e.shift)
                {
                    if (e.button == 0)
                    {
                        LeftClick(hitInfo.point, firstanchorClicked);
                        e.Use();
                    }
                    if (e.button == 1)
                    {
                        RightClick(hitInfo.point);
                        e.Use();
                    }
                }
            }
        }


        private void LeftClick(Vector3 point, bool clicked)
        {
            if (activeSetupWindow != null)
            {
                activeSetupWindow.LeftClick(point, clicked);
            }
        }


        private void RightClick(Vector3 point)
        {
            if (activeSetupWindow != null)
            {
                activeSetupWindow.RightClick(point);
            }
        }


        private void UndoAction()
        {
            firstanchorClicked = true;
            if (activeSetupWindow != null)
            {
                activeSetupWindow.UndoAction();
            }
        }


        private void Draw()
        {
            activeSetupWindow.DrawInScene();
        }

        
        #endregion
    }
}
