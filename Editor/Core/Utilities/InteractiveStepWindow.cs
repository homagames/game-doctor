using System;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public class InteractiveStepWindow : EditorWindow
    {
        private StepBasedIssue _issue;

        public static InteractiveStepWindow Create(StepBasedIssue stepBasedIssue)
        {
            var window = CreateInstance<InteractiveStepWindow>();
            window._issue = stepBasedIssue;
            return window;
        }

        private void OnGUI()
        {
            if (_issue == null) return;

            EditorGUILayout.LabelField($"Step {_issue.CurrentStepIndex + 1}/{_issue.StepCount}",
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(_issue.CurrentStep.Name, new GUIStyle(GUI.skin.label) {wordWrap = true});

            EditorGUILayout.BeginHorizontal();

            _issue.CurrentStep.Draw(_issue);

            EditorGUILayout.EndHorizontal();
        }

        private void Update()
        {
            Repaint();
        }
    }
}