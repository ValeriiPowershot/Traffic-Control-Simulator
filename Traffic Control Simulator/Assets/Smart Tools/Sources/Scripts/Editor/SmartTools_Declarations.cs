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
        static SmartTools instance;
        static VisualElement root;
        static SmartTools_Settings settings;
        static SmartTools_Settings.ColorScheme c;
        static SceneView sceneView;
        static Camera sceneCamera;
        static SmartTools_SceneData sceneData;
        static Mesh gridMesh;
        static Material gridMaterial;


        // Functional Variables
        static bool  showGrid = true;
        static bool  showSettings = false;
        static float viewDependency;
        static float moveAreaSize;
        static float gridSize        = 1;

        static bool  moveIncrement   = true;
        static float moveIncrementA  = 1;
        static float moveIncrementB  = 4;

        static bool  pivot           = false;
        static bool  rotIncrement    = true;
        static float rotIncrementA   = 22.5f;
        static float rotIncrementB   = 90.0f;
        static int   rotAxis         = 1;
        static bool  focus           = false;
        static float horizontalMode;

        static bool  snap            = true;
        static bool  snapPos         = true;
        static bool  snapPosX        = true;
        static bool  snapPosY        = true;
        static bool  snapPosZ        = true;
        static bool  snapRot         = true;
        static bool  snapRotX        = true;
        static bool  snapRotY        = true;
        static bool  snapRotZ        = true;
        static bool  snapScl         = true;
        static bool  snapSclX        = true;
        static bool  snapSclY        = true;
        static bool  snapSclZ        = true;
        static bool  cComp           = false;


        // Derived Variables
        static float moveDistance;
        static float rotationAmount;

        // UI Variables
        static Rect[] r;
        static float padding;
        static float spacing;
        static float w;
        static float h;
        static float newline;
        static float spacer;

        static readonly string uc_Settings = "?"; //"≡"; // "\u2261"; 
        static readonly string uc_Grid     = "●"; // "\u25CF";
        static readonly string uc_Pivot    = "○"; // "\u25CB";
        static readonly string uc_ArrowL   = "◄"; // "\u25C4";
        static readonly string uc_ArrowU   = "▲"; // "\u25B2";
        static readonly string uc_ArrowR   = "►"; // "\u25BA";
        static readonly string uc_ArrowD   = "▼"; // "\u25BC";
        static readonly string uc_Preset   = "◊"; // "\u25CA";

        // strings for the view and selection preset buttons
        static string s00;
        static string s01;
        static string s10;
        static string s11;

        // Visual Elements
        static MyButton   tgl_GridVisiblity;
        static MyButton   tgl_Settings;
        static Slider     sld_ViewDependency;
        static Slider     sld_MoveAreaSize;
        static MyButton   btn_GridSize;
        static FloatField flf_GridSize;

        static Label      lbl_Increment;
        static MyButton   tgl_IncrementA;
        static FloatField flf_IncrementA;
        static MyButton   tgl_IncrementB;
        static FloatField flf_IncrementB;

        static Label      lbl_Angle;
        static MyButton   tgl_Pivot;
        static MyButton   tgl_AngleA;
        static FloatField flf_AngleA;
        static MyButton   tgl_AngleB;
        static FloatField flf_AngleB;
        static MyButton   tgl_RotAxisX;
        static MyButton   tgl_RotAxisY;
        static MyButton   tgl_RotAxisZ;
                      
        static MyButton   btn_RotLeft;
        static MyButton   btn_MoveForward;
        static MyButton   btn_RotRight;
        static MyButton   btn_MoveLeft;
        static MyButton   btn_MoveBack;
        static MyButton   btn_MoveRight;
        static MyButton   btn_MoveUp;
        static MyButton   btn_MoveDown;
        static MyButton   btn_Duplicate;
        static MyButton   tgl_Focus;
        static Slider     sld_HorizontalMode;
                      
        static MyButton   tgl_Snap;
        static MyButton   tgl_SnapPos;
        static MyButton   tgl_SnapPosX;
        static MyButton   tgl_SnapPosY;
        static MyButton   tgl_SnapPosZ;
        static MyButton   tgl_SnapRot;
        static MyButton   tgl_SnapRotX;
        static MyButton   tgl_SnapRotY;
        static MyButton   tgl_SnapRotZ;
        static MyButton   tgl_SnapScl;
        static MyButton   tgl_SnapSclX;
        static MyButton   tgl_SnapSclY;
        static MyButton   tgl_SnapSclZ;
                      
        static MyButton   btn_Parent;
        static MyButton   btn_HierarchyUp;
        static MyButton   btn_HierarchyDown;
        static MyButton   btn_Unparent;
        static MyButton   tgl_CComp;
                      
        static MyButton   btn_SelPrev;
        static MyButton   btn_SelNext;
        static MyButton[] btn_Preset;

        static FloatField flf_GridOffsetX;
        static FloatField flf_GridOffsetY;
        static FloatField flf_GridOffsetZ;





        // String that is used to show notifications in the scene view
        static string notify = "";



        // Flag that a selection needs to be stored in the next editor update
        static bool storeSelection;






        //static Vector3 gridOffset;
        static bool offsetHalfGrid;


        // Used for Auto Snapping
        static Transform oldTransform;
        static Vector3 oldPosition;
        static Vector3 oldRotation;
        static Vector3 oldScale;
        static bool keySnap = false;
        static bool rotHandle = false;
        static bool vertexSnapping = false;
        static bool scaleHandler = false;


        // Used to get Double Tapping and Hotkey Hold Logic
        private static double tStamp_KeyDown;
        private static readonly float keyTapTime = 0.25f;

        // Used for Child Compensation
        static List<Transform> firstLevelChildren;
        static List<TData> tDatas;
        class TData
        {
            public Vector3 pos;
            public Quaternion rot;
            public Vector3 scl;
            public Vector3 parentScale;
            public TData(Vector3 _pos, Quaternion _rot, Vector3 _scl, Vector3 _parentScl)
            {
                pos = _pos; rot = _rot; scl = _scl; parentScale = _parentScl;
            }
        }
    }
}
