using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine.UIElements;
using UnityEditor.Events;
using UnityEditor.ShortcutManagement;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace SmartTools
{
    public partial class SmartTools
    {
        #region Various
        static void GetSceneData()
        {
            Scene scene = EditorSceneManager.GetActiveScene();
            var rootGOs = scene.GetRootGameObjects();
            string name = "##SmartTools_SceneData##";
            foreach (var go in rootGOs)
            {
                var children = go.GetComponentsInChildren<Transform>();
                foreach (var t in children)
                {
                    if (t.name == name)
                    {
                        sceneData = t.GetComponent<SmartTools_SceneData>();
                        sceneData.gameObject.hideFlags = HideFlags.HideInHierarchy;
                        //Debug.Log("Foundit!");

                        if (!SessionState.GetBool("EditorInitialized", false))
                        {
                            SessionState.SetBool("EditorInitialized", true);
                            sceneData.ClearHistory();
                        }

                        return;
                    }
                }
            }
            sceneData = SmartTools_SceneData.Init();
            //var sceneDataGO = new GameObject(name);
            //sceneData = sceneDataGO.AddComponent<SmartTools_SceneData>();
            //sceneData.gameObject.hideFlags = HideFlags.HideInHierarchy;
            //sceneData.store = new SmartTools_SceneData.StoredCombo[12];
            //for (int i = 0; i < sceneData.store.Length; i++)
            //{
            //    sceneData.store[i] = new SmartTools_SceneData.StoredCombo();
            //    sceneData.store[i].cam = new SmartTools_SceneData.StoredSceneCam();
            //    sceneData.store[i].sel = new SmartTools_SceneData.StoredSelection();
            //    sceneData.store[i].sel.objects = new Object[0];
            //}

            //if (!SessionState.GetBool("EditorInitialized", false))
            //{
            //    SessionState.SetBool("EditorInitialized", true);
            //    sceneData.ClearHistory();
            //}
        }




        static bool IsDisplayed()
        {
            if (instance == null) return false;
            return instance.displayed;
        }


        static void UpdateMoveDistance() { moveDistance = gridSize * (moveIncrement ? moveIncrementA : moveIncrementB); }
        static void UpdateRotAmount() { rotationAmount = rotIncrement ? rotIncrementA : rotIncrementB; }





        static void StoreSelection()
        {
            if (Selection.objects.Length == 0) return;

            if (sceneData.selectionHistoryIndex < 0) sceneData.selectionHistoryIndex = 0;
            if (sceneData.selectionHistoryIndex > sceneData.selectionHistory.Count - 1) sceneData.selectionHistoryIndex = sceneData.selectionHistory.Count - 1;

            if (sceneData.selectionHistory.Count > 0 && AreSelectionsEqual(Selection.objects, sceneData.selectionHistory[sceneData.selectionHistoryIndex].objects)) return;

            // Trim List - if the selection index is not at the end of the list we trim the list so the index is the last element
            if (sceneData.selectionHistoryIndex >= 0 && sceneData.selectionHistoryIndex < sceneData.selectionHistory.Count)
            {
                sceneData.selectionHistory.RemoveRange(sceneData.selectionHistoryIndex + 1, sceneData.selectionHistory.Count - (sceneData.selectionHistoryIndex + 1));
            }

            var sel = new SmartTools_SceneData.StoredSelection();
            sel.SaveSel();
            sceneData.selectionHistory.Add(sel);
            if (sceneData.selectionHistory.Count > 50) sceneData.selectionHistory.RemoveAt(0);
            sceneData.selectionHistoryIndex = sceneData.selectionHistory.Count - 1;

            if (sceneData.selectionHistory.Count > SmartTools_SceneData.selectionHistoryCapacity)
            {
                sceneData.selectionHistory.RemoveAt(0);
            }
        }



        private static bool AreSelectionsEqual(Object[] selectionA, Object[] selectionB)
        {
            if (selectionA.Length != selectionB.Length) return false;
            for (int i = 0; i < selectionA.Length; i++)
            {
                if (selectionA[i] != selectionB[i]) return false;
            }
            return true;
        }



        static string GetString(bool a, bool b)
        {
            if (!a && !b) return s00;
            else if (!a && b) return s01;
            else if (a && !b) return s10;
            else if (a && b) return s11;
            else return "";
        }


        static void UpdateAllPresetButtons()
        {
            int count = btn_Preset.Length;
            for (int i = 0; i < count; i++) UpdatePresetButton(i);
        }
        static void UpdatePresetButton(int index)
        {
            //Debug.Log(sceneData == null ? "sceneData is null" : "sceneData is not null");
            //Debug.Log(sceneData.store == null ? "sceneData.store is null" : "sceneData.store is not null");
            //Debug.Log(sceneData.store[index] == null ? "sceneData.store[" + index + "] is null" : "sceneData.store[" + index + "] is not null");
            //Debug.Log(sceneData.store[index].cam == null ? "sceneData.store[" + index + "].cam is null" : "sceneData.store[" + index + "].cam is not null");
            //Debug.Log(sceneData.store[index].sel == null ? "sceneData.store[" + index + "].sel is null" : "sceneData.store[" + index + "].sel is not null");
            var store = sceneData.store[index];
            bool hasCam = store.cam.isSaved;
            bool hasSel = store.sel.isSaved;
            btn_Preset[index].text = GetString(hasCam, hasSel);
        }


        static void LoadSelectionFromHistory(int add)
        {
            if (sceneData.selectionHistory.Count == 0) return;
            //sceneData.CleanSelectionHistory();
            sceneData.selectionHistoryIndex = Mathf.Clamp(sceneData.selectionHistoryIndex + add, 0, sceneData.selectionHistory.Count - 1);
            sceneData.selectionHistory[sceneData.selectionHistoryIndex].LoadSel();
            notify = "Selection " + (sceneData.selectionHistoryIndex + 1).ToString() + "/" + sceneData.selectionHistory.Count + (Selection.count == 0 ? "(Deleted)" : "");
        }


        static void OnPreset(int index)
        {
            var e = Event.current;
            var button = e.button;
            var modifiers = e.modifiers;
            bool shift = modifiers == EventModifiers.Shift;
            bool control = modifiers == EventModifiers.Control;
            bool none = !shift && !control;

            if (sceneData.store[index] == null) sceneData.store[index] = new SmartTools_SceneData.StoredCombo();
            var store = sceneData.store[index];

            if (e.type == EventType.MouseDown)
            {
                if (none)
                {
                    if (button == 0)
                    {
                        store.LoadSel();
                        store.LoadCam(ref sceneView);
                    }
                    else if (button == 1)
                    {
                        store.LoadCam(ref sceneView);
                    }
                    else if (button == 2)
                    {
                        store.LoadSel();
                    }
                }
                else if (shift)
                {
                    if (button == 0)
                    {
                        store.SaveSel();
                        store.SaveCam(sceneView);
                        EditorUtility.SetDirty(sceneData);
                    }
                    else if (button == 1)
                    {
                        store.SaveCam(sceneView);
                        EditorUtility.SetDirty(sceneData);
                    }
                    else if (button == 2)
                    {
                        store.SaveSel();
                        EditorUtility.SetDirty(sceneData);
                    }
                    UpdatePresetButton(index);
                }
                else if (control)
                {
                    if (button == 0)
                    {
                        store.ClearSel();
                        store.ClearCam();
                        EditorUtility.SetDirty(sceneData);
                    }
                    else if (button == 1)
                    {
                        store.ClearCam();
                        EditorUtility.SetDirty(sceneData);
                    }
                    else if (button == 2)
                    {
                        store.ClearSel();
                        EditorUtility.SetDirty(sceneData);
                    }
                    UpdatePresetButton(index);
                }
            }
            else // Must be a hotkey
            {
                store.LoadSel();
                store.LoadCam(ref sceneView);
            }
        }
        #endregion



        #region Snap and Reset
        static public void AutoSnapping()
        {
            if (Event.current == null) return;

            int controlID = GUIUtility.hotControl;
            keySnap = false;


            if (Selection.activeTransform == null) return;

            if (Event.current.keyCode == KeyCode.V)
            {
                vertexSnapping = true;
                if (Event.current.type == EventType.KeyUp) vertexSnapping = false;
            }


            if (snap)
            {
                if (Selection.activeTransform == oldTransform && !vertexSnapping)
                {
                    if (snapRot)
                    {
                        if (controlID != 0 && !rotHandle)
                        {
                            if (Selection.activeTransform.eulerAngles != oldRotation) rotHandle = true;
                        }
                        else if (controlID == 0 && rotHandle)
                        {
                            SnapRot(snapRotX, snapRotY, snapRotZ, rotationAmount);
                            rotHandle = false;
                        }
                    }

                    if (snapPos)
                    {
                        if (Selection.activeTransform.position != oldPosition && !rotHandle && (controlID != 0 || keySnap))
                        {
                            //if (uGUISelect == false)
                            {
                                SnapPos(snapPosX, snapPosY, snapPosZ);
                            }
                        }
                    }

                    if (snapScl)
                    {
                        if (Selection.activeTransform.localScale != oldScale && (controlID != 0 || keySnap))
                        {
                            if (Selection.transforms.Length == 1)
                            {
                                SnapScl(snapSclX, snapSclY, snapSclZ);
                                scaleHandler = false;
                            }
                            else if (controlID != 0 && !scaleHandler) scaleHandler = true;
                        }
                        else if (controlID == 0 && scaleHandler)
                        {
                            SnapScl(snapSclX, snapSclY, snapSclZ);
                            scaleHandler = false;
                        }
                    }
                }
                GetOldPSR();
            }
            oldTransform = Selection.activeTransform;
        }



        static void GetOldPSR()
        {
            if (Selection.activeTransform != null)
            {
                if (!rotHandle)
                {
                    oldPosition = Selection.activeTransform.position;
                    // if (Tools.current == Tool.Rect) snazzy.oldHandleRect = Tools.handleRect;
                }
                oldScale = Selection.activeTransform.localScale;
                oldRotation = Selection.activeTransform.eulerAngles;
            }
        }



        static void SnapTransform()
        {
            if (snapPos) SnapPos(snapPosX, snapPosY, snapPosZ);
            if (snapRot) SnapRot(snapRotX, snapRotY, snapRotZ, rotationAmount);
            if (snapScl) SnapScl(snapSclX, snapSclY, snapSclZ);
        }



        static void SnapPos(bool x, bool y, bool z)
        {
            //if (uGUISelect == true)
            //{
            //    if (Tools.handlePosition.x != pivotOld.x || Tools.handlePosition.y != pivotOld.y) { pivotOld = Tools.handlePosition; return; }
            //    pivotOld = Tools.handlePosition;
            //}

            Undo.RecordObjects(Selection.transforms, "Selection Snap Transform");
            foreach (Transform transform in Selection.transforms)
            {
                //if (transform == snazzySettings.offsetObject) continue;
                transform.position = SnapVector3(transform.position - sceneData.gridOffset, x, y, z, gridSize) + sceneData.gridOffset;
            }
        }



        static void SnapRot(bool x, bool y, bool z, float rotation)
        {
            Vector3 offset = Vector3.zero;
            //if (snazzySettings.offsetObject != null) offset = snazzySettings.offsetObject.eulerAngles;

            foreach (Transform transform in Selection.transforms)
            {
                SnapRot(transform, offset, rotation, x, y, z);
            }
            ResetHandler();

            void SnapRot(Transform transform, Vector3 offset, float rotation, bool snapX, bool snapY, bool snapZ)
            {
                Vector3 snapRotation = new Vector3();

                if (snapX) snapRotation.x = (Mathf.Round((transform.eulerAngles.x - offset.x) / rotation) * rotation) + offset.x; else snapRotation.x = transform.eulerAngles.x;
                if (snapY) snapRotation.y = (Mathf.Round((transform.eulerAngles.y - offset.y) / rotation) * rotation) + offset.y; else snapRotation.y = transform.eulerAngles.y;
                if (snapZ) snapRotation.z = (Mathf.Round((transform.eulerAngles.z - offset.z) / rotation) * rotation) + offset.z; else snapRotation.z = transform.eulerAngles.z;
                transform.localEulerAngles = snapRotation;
            }
        }



        static void SnapScl(bool x, bool y, bool z)
        {
            // Debug.Log ("Scale "+x+","+y+","+z);
            Vector3 scale;

            foreach (Transform transform in Selection.transforms)
            {
                scale = SnapVector3(transform.localScale, x, y, z, gridSize);
                if (x && scale.x < gridSize) { scale.x = gridSize; }
                if (y && scale.y < gridSize) { scale.y = gridSize; }
                if (z && scale.z < gridSize) { scale.z = gridSize; }
                transform.localScale = scale;
            }
        }



        static Vector3 SnapVector3(Vector3 snapVector3, bool snapX, bool snapY, bool snapZ, float amount)
        {
            if (snapX) snapVector3.x = Mathf.Round(snapVector3.x / amount) * amount;
            if (snapY) snapVector3.y = Mathf.Round(snapVector3.y / amount) * amount;
            if (snapZ) snapVector3.z = Mathf.Round(snapVector3.z / amount) * amount;
            return snapVector3;
        }



        static void ResetTransform(bool fullReset = false)
        {
            if (fullReset)
            {
                ResetPos(true, true, true);
                ResetRot(true, true, true);
                ResetScl(true, true, true);
                return;
            }
            if (snapPos) ResetPos(snapPosX, snapPosY, snapPosZ);
            if (snapRot) ResetRot(snapRotX, snapRotY, snapRotZ);
            if (snapScl) ResetScl(snapSclX, snapSclY, snapSclZ);
        }



        static void ResetPos(bool x, bool y, bool z)
        {
            Vector3 position = new Vector3();
            Undo.RecordObjects(Selection.transforms, "Reset Position");

            foreach (Transform transform in Selection.transforms)
            {
                position = transform.position;
                if (x) position.x = 0;
                if (y) position.y = 0;
                if (z) position.z = 0;
                transform.localPosition = position;
            }
        }



        static void ResetRot(bool x, bool y, bool z)
        {
            Vector3 rotation = new Vector3();
            Undo.RecordObjects(Selection.transforms, "Reset Rotation");

            foreach (Transform transform in Selection.transforms)
            {
                rotation = transform.localEulerAngles;
                if (x) rotation.x = 0;
                if (y) rotation.y = 0;
                if (z) rotation.z = 0;
                transform.localEulerAngles = rotation;
            }
            ResetHandler();
        }



        static void ResetScl(bool x, bool y, bool z)
        {
            Vector3 scale = new Vector3();

            Undo.RecordObjects(Selection.transforms, "Selection Reset Scale");
            foreach (Transform transform in Selection.transforms)
            {
                scale = transform.localScale;
                if (x) scale.x = 1;
                if (y) scale.y = 1;
                if (z) scale.z = 1;

                transform.localScale = scale;
            }
        }



        static void ResetHandler()
        {
            if (Selection.activeTransform != null)
            {
                if (Tools.pivotRotation == PivotRotation.Global) Tools.handleRotation = Quaternion.identity;
                else                                             Tools.handleRotation = Selection.activeTransform.rotation;
            }
        }



        static void FlipScale(bool x, bool y, bool z)
        {
            foreach (Transform t in Selection.transforms)
            {
                var scl = t.localScale;
                if (x) scl.x = -scl.x;
                if (y) scl.y = -scl.y;
                if (z) scl.z = -scl.z;
                t.localScale = scl;
            }
        }



        static void MultiplyScale(float x, float y, float z)
        {
            foreach (Transform t in Selection.transforms)
            {
                t.localScale = new Vector3(t.localScale.x * x, t.localScale.y * y, t.localScale.z * z);
            }
        }
        #endregion



        #region Moving
        static void MoveSelection(int dirIndex)
        {
            if (Selection.activeTransform == null) return;

            var dir = Vector3.zero;
            var t = sceneCamera.transform;
            if      (dirIndex == 0) dir = SnapToClosestAxis( t.right                   );
            else if (dirIndex == 1) dir = SnapToClosestAxis(-t.right                   );
            else if (dirIndex == 2) dir = SnapToClosestAxis( t.up     ,  horizontalMode);
            else if (dirIndex == 3) dir = SnapToClosestAxis(-t.up     ,  horizontalMode);
            else if (dirIndex == 4) dir = SnapToClosestAxis( t.forward, -horizontalMode);
            else if (dirIndex == 5) dir = SnapToClosestAxis(-t.forward, -horizontalMode);
            dir *= moveDistance;
            Undo.RecordObjects(Selection.transforms, "Move Selection");
            foreach (Transform transform in Selection.transforms) transform.position += dir;
        }

        static Vector3 SnapToClosestAxis(Vector3 direction, float biasY = 0) // TODO: Solve Bias Bug (Up and Forward can be the same when using a bias) also the movement does not seem to work in local axis
        {
            float x = Mathf.Abs(direction.x);
            float y = Mathf.Abs(direction.y) + biasY;
            float z = Mathf.Abs(direction.z);
            if      (x > y && x > z) return new Vector3(Mathf.Sign(direction.x), 0, 0);  // Snap to X
            else if (y > x && y > z) return new Vector3(0, Mathf.Sign(direction.y), 0);  // Snap to Y
            else                     return new Vector3(0, 0, Mathf.Sign(direction.z));  // Snap to Z
        }

        static bool GetHorizontalMode()
        {
            if (sceneCamera != null) return SnapToClosestAxis(sceneCamera.transform.up, horizontalMode).y == 0;
            return false;
        }
        #endregion



        #region Rotation
        static void RotateSelection(int sign)
        {
            if (Selection.activeTransform == null) return;

            Undo.RecordObjects(Selection.transforms, "Rotate Selection");
            if (pivot) Rotate(rotationAmount * sign); else RotateAround(rotationAmount * sign);
            //if (snap && snapRot) SnapRot(rotationAmount, snapRotX, snapRotY, snapRotZ); // TODO: Decide if this should be here, its not done on moving either
        }

        static void Rotate(float rotation)
        {
            var space = Tools.pivotRotation == PivotRotation.Global ? Space.World : Space.Self;
            var axis = Vector3.zero;
            switch (rotAxis)
            {
                case 0: axis = new Vector3(rotation, 0, 0); break;
                case 1: axis = new Vector3(0, rotation, 0); break;
                case 2: axis = new Vector3(0, 0, rotation); break;
            }
            foreach (Transform transform in Selection.transforms) transform.Rotate(axis, space);
            ClampRotation();
        }

        static void RotateAround(float rotation)
        {
            bool global = Tools.pivotRotation == PivotRotation.Global;
            var t = Selection.activeTransform;

            Vector3 point = Tools.handlePosition;
            var axis = Vector3.zero;
            if      (rotAxis == 0) axis = global ? Vector3.right   : t.right;
            else if (rotAxis == 1) axis = global ? Vector3.up      : t.up;
            else if (rotAxis == 2) axis = global ? Vector3.forward : t.forward;

            foreach (Transform transform in Selection.transforms) transform.RotateAround(point, axis, rotation);
            ClampRotation();
        }

        static void ClampRotation() // Wraps numbers around, to avoid angles larger than 360
        {
            Vector3 rotation;
            foreach (Transform transform in Selection.transforms)
            {
                rotation = transform.localEulerAngles;
                rotation.x = Mathf.DeltaAngle(0, rotation.x);
                rotation.y = Mathf.DeltaAngle(0, rotation.y);
                rotation.z = Mathf.DeltaAngle(0, rotation.z);
                transform.localEulerAngles = rotation;
            }
        }
        #endregion



        #region Child Compensation
        static void OnChildCompensationToggled()
        {
            if (!cComp) return;

            if (firstLevelChildren == null) firstLevelChildren = new List<Transform>();
            else firstLevelChildren.Clear();

            if (tDatas == null) tDatas = new();
            else tDatas.Clear();

            Transform[] selectedTransforms = Selection.transforms;
            foreach (Transform selected in selectedTransforms)
            {
                // Get all direct children of the selected transforms
                for (int i = 0; i < selected.childCount; i++)
                {
                    Transform child = selected.GetChild(i);
                    firstLevelChildren.Add(child);
                }
            }

            foreach (var t in firstLevelChildren)
            {
                tDatas.Add(new TData(t.position, t.rotation, t.localScale, t.parent.localScale));
            }
        }

        static void ChildCompensationUpdate()
        {
            if (!cComp) return;

            var childCount = firstLevelChildren.Count;
            for (int i = 0; i < childCount; i++)
            {
                var t = firstLevelChildren[i];
                var data = tDatas[i];
                var ds = new Vector3(t.parent.localScale.x / data.parentScale.x, t.parent.localScale.y / data.parentScale.y, t.parent.localScale.z / data.parentScale.z);
                Undo.RecordObject(t, "Child Conpensation Undo");
                t.SetPositionAndRotation(data.pos, data.rot);
                t.localScale = new Vector3(data.scl.x / ds.x, data.scl.y / ds.y, data.scl.z / ds.z);
            }
        }
        #endregion



        #region Hierarchy UP/Down
        static void SelectParent()
        {
            if (Selection.activeTransform == null) return;

            Undo.RecordObjects(Selection.transforms, "Select Parent");
            HashSet<GameObject> parents = new();
            foreach (Transform t in Selection.transforms)
            {
                if (t.parent != null) parents.Add(t.parent.gameObject);
                else parents.Add(t.gameObject);
            }
            GameObject[] array = new GameObject[parents.Count];
            parents.CopyTo(array);
            Selection.objects = array;
        }

        static void SelectChildren()
        {
            if (Selection.activeTransform == null) return;

            List<GameObject> children = new List<GameObject>();
            bool changed = false;
            foreach (Transform t in Selection.transforms)
            {
                if (t.childCount == 0) children.Add(t.gameObject);
                else
                {
                    changed = true;
                    for (int i = 0; i < t.childCount; i++)
                    {
                        Transform child = t.GetChild(i);
                        children.Add(child.gameObject);
                    }
                }
            }
            if (children.Count > 0 && changed)
            {
                Undo.RecordObjects(Selection.transforms, "Select Children");
                Selection.objects = children.ToArray();
            }
        }
        #endregion



        #region Parenting
        static void CreateParentAtOrigin() // Create Parent at Origin
        {
            if (Selection.transforms.Length == 0) return;
            tgl_CComp.UpdateToggle(cComp = false);

            GameObject go = new GameObject("GameObject");
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            go.transform.parent = Selection.activeTransform.parent;

            foreach (Transform transform in Selection.transforms)
            {
                Undo.SetTransformParent(transform, go.transform, "Change Parent " + transform.name);
            }
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy"); // Focus on the hierarchy window
            EditorApplication.delayCall += () => {
                Selection.activeObject = go; // Ensure the object is still selected
                EditorWindow.focusedWindow.SendEvent(new Event { keyCode = KeyCode.F2, type = EventType.KeyDown });
            };
        }

        static void CreateParentAtCenter() // Create Parent at Center
        {
            if (Selection.transforms.Length == 0) return;
            tgl_CComp.UpdateToggle(cComp = false);

            GameObject go = new GameObject("GameObject");
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            go.transform.parent = Selection.activeTransform.parent;
            go.transform.position = GetCenterPosition();

            foreach (Transform transform in Selection.transforms)
            {
                Undo.SetTransformParent(transform, go.transform, "Change Parent " + transform.name);
            }
            //MoveParentToCenter(go.transform);
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy"); // Focus on the hierarchy window
            EditorApplication.delayCall += () => {
                Selection.activeObject = go; // Ensure the object is still selected
                EditorWindow.focusedWindow.SendEvent(new Event { keyCode = KeyCode.F2, type = EventType.KeyDown });
            };
        }

        static void MoveParentToCenter(Transform parent)
        {
            if (Selection.activeTransform == null)
            {
                notify = "Nothing selected";
                return;
            }
            if (parent == null)
            {
                notify = "Active selection does not have a parent";
                return;
            }
            tgl_CComp.UpdateToggle(cComp = false);

            Undo.RecordObject(parent, "Move parent to center");
            Undo.RecordObjects(Selection.transforms, "Move parent to center");
            var center = GetCenterPosition();
            var transforms = Selection.transforms;
            foreach (Transform t in transforms) t.parent = null;
            parent.position = center;
            //if (snap && snapPos) parent.position = SnapVector3(parent.position, snapPosX, snapPosY, snapPosY, moveDistance);
            foreach (Transform t in transforms) t.parent = parent;

            notify = parent.name + " moved to center of selection";
        }

        static void UnparentSelection(bool makeSiblingToParent = false)
        {
            if (Selection.activeTransform == null) return;

            foreach (Transform t in Selection.transforms)
            {
                if (t.parent == null) continue;
                Transform newParent = null;
                if (t.parent.parent != null && makeSiblingToParent) newParent = t.parent.parent; 
                Undo.SetTransformParent(t, newParent, "Parent change " + t.name);
            }
        }

        static Vector3 GetCenterPosition() // Actually its average Pos
        {
            Vector3 center = new Vector3(0, 0, 0);
            foreach (Transform t in Selection.transforms) center += t.position;
            return center / Selection.transforms.Length;
        }
        #endregion
    }

    public class CustomTextureImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            
            // Check if the imported asset has the custom extension
            if (assetPath.Contains("/.HiddenFolder/"))
            {
                Debug.Log("smarttoolstex");
                TextureImporter importer = (TextureImporter)assetImporter;

                // Set texture import settings exactly as per the screenshot
                importer.textureType = TextureImporterType.Default;
                importer.textureShape = TextureImporterShape.Texture2D;
                importer.sRGBTexture = true;
                importer.alphaSource = TextureImporterAlphaSource.None;
                importer.alphaIsTransparency = false;
                //importer.removePsdAlpha = false;

                // Advanced settings
                importer.npotScale = TextureImporterNPOTScale.ToNearest;
                importer.isReadable = false;
                importer.streamingMipmaps = false;
                importer.mipmapEnabled = true;
                importer.borderMipmap = false;
                importer.mipMapsPreserveCoverage = true;
                importer.alphaTestReferenceValue = 0.5f;
                importer.mipmapFilter = TextureImporterMipFilter.KaiserFilter;
                importer.wrapMode = TextureWrapMode.Repeat;
                importer.filterMode = FilterMode.Trilinear;
                importer.anisoLevel = 16;

                // Platform settings
                TextureImporterPlatformSettings platformSettings = new TextureImporterPlatformSettings
                {
                    maxTextureSize = 2048,
                    resizeAlgorithm = TextureResizeAlgorithm.Mitchell,
                    format = TextureImporterFormat.Automatic,
                    compressionQuality = (int)TextureImporterCompression.Uncompressed
                };

                importer.SetPlatformTextureSettings(platformSettings);
            }
        }
    }
}
