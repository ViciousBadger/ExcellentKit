using UnityEngine;

namespace ExcellentKit
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public static class GizmoLabelClearer
    {
        static GizmoLabelClearer()
        {
            EditorApplication.update += Update;
        }

        static void Update()
        {
            GizmosExtra.ClearLabelOffsets();
        }
    }

    public static class GizmosExtra
    {
        private const float MAX_VIEW_DIST = 15f;
        private const float MIN_VIEW_DIST = 5f;

        private static bool IsInViewAndSetAlpha(Vector3 position)
        {
            var delta = position - Camera.current.transform.position;

            if (delta.magnitude < MAX_VIEW_DIST)
            {
                var angle = Vector3.Angle(delta, Camera.current.transform.forward);
                if (angle < 90f)
                {
                    var alpha = delta.magnitude.Remap(MIN_VIEW_DIST, MAX_VIEW_DIST, 1f, 0f);
                    var guiColor = GUI.color;
                    guiColor.a = alpha;
                    GUI.color = guiColor;

                    var gizmoColor = Gizmos.color;
                    gizmoColor.a = alpha;
                    Gizmos.color = gizmoColor;
                    return true;
                }
            }
            return false;
        }

        private static readonly Dictionary<Vector3Int, float> LabelOffsetAtVector = new();

        public static void ClearLabelOffsets()
        {
            LabelOffsetAtVector.Clear();
        }

        public static void DrawLabel(Vector3 position, string text)
        {
            if (IsInViewAndSetAlpha(position))
            {
                var style = new GUIStyle
                {
                    fontSize = 10,
                    alignment = TextAnchor.LowerCenter,
                    normal = { textColor = Gizmos.color, background = Bg() },
                    padding = new RectOffset(6, 6, 4, 4),
                };

                var vectorInGrid = Vector3Int.RoundToInt(position * 4f);
                if (!LabelOffsetAtVector.ContainsKey(vectorInGrid))
                {
                    LabelOffsetAtVector.Add(vectorInGrid, 0f);
                }

                // Determine label position.
                var content = new GUIContent(text);
                var guiPosition = HandleUtility.WorldToGUIPoint(position);
                guiPosition.y += 24; // Initial offset to prevent drawing over icons
                guiPosition.y += LabelOffsetAtVector[vectorInGrid];
                var size = style.CalcSize(content);
                var rect = new Rect(guiPosition.x - size.x / 2f, guiPosition.y, size.x, size.y);
                LabelOffsetAtVector[vectorInGrid] += size.y + 4;

                Handles.BeginGUI();
                GUI.Label(rect, content, style);
                Handles.EndGUI();

                //Handles.Label(offsetPosition, text, style);

                // Draw an invisible thingy that makes the object selectable
                // by clicking the label
                //Gizmos.DrawIcon(offsetPosition, "Selector.png", false);
            }
        }

        const float ARROWHEAD_LENGTH = 0.1f;
        const float ARROWHEAD_ANGLE = 20f;

        public static void DrawArrow(
            Vector3 start,
            Vector3 end,
            string label = null,
            float arrowPosition = 0.3f
        )
        {
            Vector3 arrowTip = Vector3.Lerp(start, end, arrowPosition);

            if (Vector3.Distance(start, end) > 0.01f)
            {
                Gizmos.DrawLine(start, end);

                var delta = end - start;

                Vector3 right =
                    (
                        Quaternion.LookRotation(delta)
                        * Quaternion.Euler(ARROWHEAD_ANGLE, 0, 0)
                        * Vector3.back
                    ) * ARROWHEAD_LENGTH;
                Vector3 left =
                    (
                        Quaternion.LookRotation(delta)
                        * Quaternion.Euler(-ARROWHEAD_ANGLE, 0, 0)
                        * Vector3.back
                    ) * ARROWHEAD_LENGTH;
                Vector3 up =
                    (
                        Quaternion.LookRotation(delta)
                        * Quaternion.Euler(0, ARROWHEAD_ANGLE, 0)
                        * Vector3.back
                    ) * ARROWHEAD_LENGTH;
                Vector3 down =
                    (
                        Quaternion.LookRotation(delta)
                        * Quaternion.Euler(0, -ARROWHEAD_ANGLE, 0)
                        * Vector3.back
                    ) * ARROWHEAD_LENGTH;

                Gizmos.DrawRay(arrowTip, right);
                Gizmos.DrawRay(arrowTip, left);
                Gizmos.DrawRay(arrowTip, up);
                Gizmos.DrawRay(arrowTip, down);
            }

            if (label != null)
            {
                DrawLabel(arrowTip, label);
            }
        }

        public static void DrawCircle(Vector3 center, Vector3 axis, float radius)
        {
            const int sECTIONS = 16;

            Vector3 prevPos = Vector3.zero;

            for (int i = 0; i <= sECTIONS; i++)
            {
                float phi = i / (float)sECTIONS * Mathf.PI * 2f;

                var rot = Quaternion.LookRotation(axis) * Quaternion.Euler(90, 0, 0);

                var pos = center + rot * new Vector3(Mathf.Cos(phi), 0f, Mathf.Sin(phi)) * radius;

                if (prevPos != Vector3.zero)
                {
                    Gizmos.DrawLine(prevPos, pos);
                }

                prevPos = pos;
            }
        }

        private static Texture2D cachedBgTex;

        public static Texture2D Bg()
        {
            if (cachedBgTex == null)
            {
                cachedBgTex = MakeTex(16, 16, Color.black);
            }
            return cachedBgTex;
        }

        private static Texture2D MakeTex(int width, int height, Color32 col)
        {
            Color32[] pix = new Color32[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new(width, height);
            result.SetPixels32(pix);
            result.Apply();
            return result;
        }

        public static void ColorPaletteSignalReciever()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 1f);
        }

        public static void ColorPaletteSignalBehaviour()
        {
            Gizmos.color = new Color(0f, 0.5f, 1f);
        }

        public static void ColorPaletteSignalEmitter()
        {
            Gizmos.color = new Color(0.5f, 0.2f, 0.9f);
        }

        public static void ColorPaletteSignalPipe()
        {
            Gizmos.color = new Color(0.2f, 1.0f, 0.2f);
        }

        public static void ColorPaletteGameplay()
        {
            Gizmos.color = new Color(1f, 0.4f, 0.4f);
        }

        public static void ColorPaletteError()
        {
            Gizmos.color = new Color(1f, 0.0f, 0.0f);
        }

        public static void ColorPaletteLegacy()
        {
            Gizmos.color = new Color(0.3f, 0.3f, 0.3f);
        }

        public static void ColorPaletteDialogue()
        {
            Gizmos.color = new Color(0.3f, 1.0f, 0.5f);
        }

        public static void ColorPaletteMarker()
        {
            Gizmos.color = new Color(0.6f, 0.8f, 1.0f);
        }
    }
}
