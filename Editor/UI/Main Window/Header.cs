using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class SplitViewWindow
    {
        private bool AutoFixToggle;
        private float HeaderSize;
        
        private void DrawHeader()
        {
            int textSize = 28;
            int padding = Mathf.RoundToInt(EditorGUIUtility.singleLineHeight / 2);
            GUILayout.BeginHorizontal(new GUIStyle
            {
                margin = new RectOffset(padding, padding, padding, padding)
            });

            GUIContent titleGuiContent = new GUIContent("Game Doctor");
            var titleGuiStyle = new GUIStyle(EditorStyles.largeLabel) { fontStyle = FontStyle.Bold, fontSize = textSize };
            GUILayout.Label(titleGuiContent, titleGuiStyle);
            Rect labelRect = GUILayoutUtility.GetRect(titleGuiContent, titleGuiStyle); 
        
            GUILayout.FlexibleSpace();

            GUIStyle controlStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16,
                margin = {top = 10}
            };

            AutoFixToggle = GUILayout.Toggle(AutoFixToggle, "Auto-Fix", new GUIStyle(controlStyle) {margin = {right = 0}});
            if (GUILayout.Button("Scan", new GUIStyle(controlStyle) {margin = {left = 0}}))
            {
                Scan();
            }
            GUILayout.Space(15);
        
            GUI.enabled = false;
            GUILayout.Button("Fix Auto. issues", new GUIStyle(controlStyle));
            GUI.enabled = true;
        
            GUILayout.EndHorizontal();
            EditorGUILayoutExtension.DrawHorizontalSeparator(1);

            if (Event.current.type == EventType.Repaint) 
                HeaderSize = Mathf.CeilToInt(labelRect.height) + 2 * padding + titleGuiStyle.margin.top + titleGuiStyle.margin.bottom;
        }

        private async Task Scan()
        {
            List<ICheck> checks = Profile.CheckList;

            float maxI = checks.Count;
            string scanWindowTitle = "Executing all checks";
            string scanWindowContent = "Looking for issues in the project";

            try
            {
                EditorUtility.DisplayProgressBar(scanWindowTitle, scanWindowContent, 0);
                for (int i = 0; i < maxI; i++)
                {
                    try
                    {
                        await checks[i].Execute();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Exception when running \"{checks[i].Name}\" check:\n{e}");
                    }
                    EditorUtility.DisplayProgressBar(scanWindowTitle, scanWindowContent, i / maxI);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}
