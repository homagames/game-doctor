using System.Linq;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class GameDoctorWindow
    {
        private const int HeaderTextSize = 28;
        
        private bool AutoFixToggle;
        private float HeaderSize;
        
        private void DrawHeader()
        {
            int padding = Mathf.RoundToInt(EditorGUIUtility.singleLineHeight / 2);
            GUILayout.BeginHorizontal(new GUIStyle
            {
                margin = new RectOffset(padding, padding, padding, padding)
            });

             DrawLabel(out var labelRect, out var titleGuiStyle);

            GUILayout.FlexibleSpace();

            GUIStyle controlStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16,
                margin = {top = 10}
            };

            AutoFixToggle = GUILayout.Toggle(AutoFixToggle, "Auto-Fix", new GUIStyle(controlStyle) {margin = {right = 0}});
            if (GUILayout.Button("Run all Checks", new GUIStyle(controlStyle) {margin = {left = 0}}))
            {
                if (AutoFixToggle)
                    RunAllChecksAndFix();
                else
                    RunAllChecks();
            }
            
            GUILayout.Space(15);

            EditorGUI.BeginDisabledGroup(! GetAllIssues().Any(issue =>
                issue.AutomationType == AutomationType.Automatic && !GetUiData(issue).Fixed));
            if (GUILayout.Button("Fix Auto. issues", new GUIStyle(controlStyle)))
            {
                FixAllAutoIssues();
            }
            EditorGUI.EndDisabledGroup();
        
            GUILayout.EndHorizontal();
            
            EditorGUILayoutExtension.DrawHorizontalSeparator(1);

            if (Event.current.type == EventType.Repaint) 
                HeaderSize = Mathf.CeilToInt(labelRect.height) + 2 * padding + titleGuiStyle.margin.top + titleGuiStyle.margin.bottom;
        }

        private static void DrawLabel(out Rect labelRect, out GUIStyle titleGuiStyle)
        {
            GUIContent titleGuiContent = new GUIContent("Game Doctor");
            titleGuiStyle = new GUIStyle(EditorStyles.largeLabel) {fontStyle = FontStyle.Bold, fontSize = HeaderTextSize};
            GUILayout.Label(titleGuiContent, titleGuiStyle);
            labelRect = GUILayoutUtility.GetRect(titleGuiContent, titleGuiStyle);
        }
    }
}
