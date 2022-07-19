using System.Linq;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class GameDoctorWindow
    {
        private bool AutoFixToggle;
        private float HeaderSize => EditorGUILayoutExtension.ToolBarHeight * 2;

        private void DrawHeader()
        {
            EditorGUILayoutExtension.BeginToolBar();

            AutoFixToggle = EditorGUILayoutExtension.ToolBarToggle(AutoFixToggle, "Auto-Fix");
            if (EditorGUILayoutExtension.ToolBarButton("Run all Checks"))
            {
                if (AutoFixToggle)
                    RunAllChecksAndFix();
                else
                    RunAllChecks();
            }

            EditorGUI.BeginDisabledGroup(!GetAllIssues().Any(issue =>
                issue.AutomationType == AutomationType.Automatic && !GetUiData(issue).Fixed));
            if (EditorGUILayoutExtension.ToolBarButton("Fix Auto. issues"))
            {
                FixAllAutoIssues();
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayoutExtension.EndToolBar();
            
            
            EditorGUILayoutExtension.BeginToolBar();
            SearchString = EditorGUILayoutExtension.ToolBarSearchBar(SearchString, GUILayout.ExpandWidth(true));
            HideFixed = EditorGUILayoutExtension.ToolBarToggle(HideFixed, "Hide fixed");
            EditorGUILayoutExtension.EndToolBar();

        }
    }
}
