using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace XanderDevelops.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteAlways]
    public class CurvedText : MonoBehaviour
    {
        private TMP_Text textComponent;  // Text component to curve

        [SerializeField, Range(-100f, 100f)]
        [Tooltip("Radius of the curve (higher values = less curve)")]
        private float curve = 30;

        [SerializeField, Range(20f, 100f)]
        [Tooltip("Fixed arc length per character (spacing between characters along the curve)")]
        private float spacing = 30f;

        [SerializeField]
        //[Tooltip("Threshold beyond which the text appears straight (no curvature)")]
        private float flatnessThreshold = 4000f;

        [SerializeField, Range(-360f, 360f)]
        [Tooltip("Offset to rotate the arc around the center (in degrees)")]
        private float angularOffset = 0f;

        private TMP_TextInfo textInfo;
        private float radius;
        private string lastText;
        private float lastCurve, lastSpacing, lastAngularOffset, lastFlatnessThreshold;

        private void Awake()
        {
            textComponent = GetComponent<TMP_Text>();
        }        
        
        private void OnEnable()
        {
            textComponent = GetComponent<TMP_Text>();

            // Ensure we only subscribe once
            if (textComponent != null)
            {
                textComponent.OnPreRenderText -= ModifyTextMesh;
                textComponent.OnPreRenderText += ModifyTextMesh;
            }
        }

        private void OnDisable()
        {
            if (textComponent != null)
            {
                textComponent.OnPreRenderText -= ModifyTextMesh;
            }
        }

        // This function is called automatically by Unity when any value in the Inspector is changed.
        private void OnValidate()
        {
            if (textComponent == null) textComponent = GetComponent<TMP_Text>();

            if (textComponent != null)
            {
                // Marking the text as dirty triggers the OnPreRenderText event automatically
                textComponent.SetAllDirty();
            }
        }

        private void ModifyTextMesh(TMP_TextInfo textInfo)
        {
            // Prevent recursion by checking if we are already updating
            if (isUpdating) return;

            isUpdating = true;
            this.textInfo = textInfo;
            UpdateTextCurve();
            isUpdating = false;
        }

        bool isUpdating = false;

        private void UpdateTextCurve()
        {
            if (textComponent == null) return;

            if (curve != 0){
                radius = flatnessThreshold/curve;
            }else{
                radius = flatnessThreshold/0.001f;
            }

            // Force an update to text info
            textComponent.ForceMeshUpdate();
            textInfo = textComponent.textInfo;
            
            if (textInfo == null) return;
                
            int characterCount = textInfo.characterCount;
            
            if (characterCount == 0) return;

            // Get bounds of the text to determine layout
            float totalArcLength = spacing * (characterCount - 1);
            float anglePerCharacter = totalArcLength / Mathf.Abs(radius) * Mathf.Rad2Deg;

            for (int i = 0; i < characterCount; i++)
            {
                if (!textInfo.characterInfo[i].isVisible) continue;

                // Get the index and character vertices
                int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // Calculate character midpoint and offsets
                Vector3 charMidBaselinePos = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i].baseLine);
                vertices[vertexIndex + 0] -= charMidBaselinePos;
                vertices[vertexIndex + 1] -= charMidBaselinePos;
                vertices[vertexIndex + 2] -= charMidBaselinePos;
                vertices[vertexIndex + 3] -= charMidBaselinePos;

                // Calculate angle offset for each character
                float charAngle = (angularOffset + (-totalArcLength / 2f) + i * spacing) / Mathf.Abs(radius) * Mathf.Rad2Deg;

                // Check if curvature is too small
                if (Mathf.Abs(radius) > flatnessThreshold)
                {
                    // Set text straight
                    charAngle = -charAngle;
                }

                // Calculate the character's new position along the circular path
                float angleRadians = charAngle * Mathf.Deg2Rad;

                if(curve < 1) angleRadians = -angleRadians;

                Vector3 offset = new Vector3(Mathf.Sin(angleRadians) * radius, Mathf.Cos(angleRadians) * radius, 0) - new Vector3(0f, radius, 0f);

                // Handle negative radius to flip the curvature
                Quaternion rotation = Quaternion.Euler(0, 0, radius > 0 ? -charAngle : charAngle);

                // Apply the transformation matrix to the vertices
                Matrix4x4 matrix = Matrix4x4.TRS(offset, rotation, Vector3.one);

                vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
                vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
                vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
                vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
            }

            // Update the mesh with the new vertex positions
            textComponent.UpdateVertexData();
        }
    }
}