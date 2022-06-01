using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public static partial class EditorGUILayoutExtension
    {
        public static void ColorLabel(string label, Color color, params GUILayoutOption[] options)
            => ColorLabel(label, color, GUI.skin.label, options);
        public static void ColorLabel(GUIContent content, Color color, params GUILayoutOption[] options)
            => ColorLabel(content, color, GUI.skin.label, options);
        public static void ColorLabel(string label, Color color, GUIStyle style, params GUILayoutOption[] options)
            => ColorLabel(new GUIContent(label), color, style, options);
        
        public static void ColorLabel(GUIContent content, Color color, GUIStyle style, params GUILayoutOption[] options)
        {
            using (new EditorGUIExtension.ColorScope(color))
                EditorGUILayout.LabelField(content, new GUIStyle(style) {normal = { textColor = Color.white }}, options);
        }
    }
}
