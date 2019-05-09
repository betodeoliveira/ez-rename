using UnityEditor;
using UnityEngine;

namespace SMG.EzRenamer
{
    public class EzRR_Style : Editor
    {
        public const int smallBtnHeight = 15;
        public const int mediumBtnHeight = 23;
        public const int bigBtnHeight = 30;
        public static GUIStyle customStyle = new GUIStyle();
        public static Color32 uiLineColor = new Color32(89, 89, 89, 255);
        public delegate void ButtonAction();

        public static void DrawHeader(string title)
        {
            // Configurate the style to mach the foldout one
            customStyle = new GUIStyle(EditorStyles.label);
            customStyle.fontStyle = FontStyle.Bold;
            // Draw the header
            EditorGUILayout.LabelField(title, customStyle);
            // Revert the style
            customStyle = EditorStyles.label;
        }

        public static bool DrawFoldoutHeader(string title, bool show)
        {
            // Configurate the style to mach the foldout one
            customStyle = new GUIStyle(EditorStyles.foldout);
            customStyle.fontStyle = FontStyle.Bold;
            // Draw the header
            show = EditorGUILayout.Foldout(show, title, customStyle);
            customStyle = EditorStyles.foldout;
            return show;
        }

        public static void DrawUILine(Color color, int thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static void DrawColoredLabelField(Color color, string text)
        {
            customStyle = new GUIStyle(EditorStyles.label);
            customStyle.normal.textColor = color;
            EditorGUILayout.LabelField(text, customStyle);
        }

        public static bool ToggleLeftBold(string title, bool value)
        {
            customStyle = new GUIStyle(EditorStyles.label);
            customStyle.fontStyle = FontStyle.Bold;
            value = EditorGUILayout.ToggleLeft(title, value, customStyle);
            customStyle = EditorStyles.foldout;
            return value;
        }

        public static bool ToggleLeftBold(string title, bool value, float customWidth)
        {
            customStyle = new GUIStyle(EditorStyles.label);
            customStyle.fontStyle = FontStyle.Bold;
            value = EditorGUILayout.ToggleLeft(title, value, customStyle, GUILayout.Width(customWidth));
            customStyle = EditorStyles.foldout;
            return value;
        }

        public static void DrawHelpButton(string url)
        {
            // Creates the custom style to remove the button layout and leave just the texture
            customStyle = new GUIStyle();
            // Creates the button using the _Help texture from Unity
            if (GUILayout.Button(EditorGUIUtility.FindTexture("_Help"), customStyle, GUILayout.Width(20), GUILayout.Height(20)))
            {
                Application.OpenURL(url);
            }
        }

        public static void DrawFindButton(ButtonAction action)
        {
            // Creates the custom style to remove the button layout and leave just the texture
            customStyle = new GUIStyle();
            // Creates the button using the ShurikenObjectField texture from Unity
            if (GUILayout.Button(EditorGUIUtility.IconContent("ShurikenObjectField"), customStyle, GUILayout.Width(30), GUILayout.Height(20)))
            {
                action();
            }
        }
    }
}