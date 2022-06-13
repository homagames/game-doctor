using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public static partial class EditorGUILayoutExtension
    {
        public static void ColorLabel([NotNull] string label, Color color, [NotNull] params GUILayoutOption[] options)
            => ColorLabel(label, color, GUI.skin.label, options);
        public static void ColorLabel([NotNull] GUIContent content, Color color, [NotNull] params GUILayoutOption[] options)
            => ColorLabel(content, color, GUI.skin.label, options);
        public static void ColorLabel([NotNull] string label, Color color, [NotNull] GUIStyle style, [NotNull] params GUILayoutOption[] options)
            => ColorLabel(new GUIContent(label), color, style, options);
        
        public static void ColorLabel([NotNull] GUIContent content, Color color, [NotNull] GUIStyle style, [NotNull] params GUILayoutOption[] options)
        {
            using (new EditorGUIExtension.ColorScope(color))
                EditorGUILayout.LabelField(content, new GUIStyle(style) {normal = { textColor = Color.white }}, options);
        }
    }
}
