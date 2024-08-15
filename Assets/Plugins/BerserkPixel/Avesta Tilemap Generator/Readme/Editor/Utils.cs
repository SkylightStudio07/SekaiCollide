using UnityEditor;
using UnityEngine;

namespace BerserkPixel.Readme
{
    public static class Utils
    {
        public static void CreateTitle(string title)
        {
            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            DrawUILine(Color.gray, 1, 12);
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            var r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2f;
            r.x -= 2;
            EditorGUI.DrawRect(r, color);
        }
    }
}