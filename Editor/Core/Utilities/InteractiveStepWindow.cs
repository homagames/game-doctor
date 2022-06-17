using System;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public class InteractiveStepWindow : EditorWindow
    {
        private StepBasedIssue _issue;
        private static InteractiveStepWindow _window;
        public static bool IsOpen => EditorWindow.HasOpenInstances<InteractiveStepWindow>();

        public static void Begin(StepBasedIssue stepBasedIssue, Vector2 size = new Vector2())
        {
            _window = CreateInstance<InteractiveStepWindow>();
            if (size == Vector2.zero)
                size = new Vector2(400, 200);
            _window.minSize = size;
            _window.maxSize = size;
            _window.titleContent = new GUIContent("Interactive Fix");
            _window._issue = stepBasedIssue;
            _window.ShowUtility();
        }

        private void OnGUI()
        {
            if (_issue?.CurrentStep == null) return;

            EditorGUILayout.LabelField($"Step {_issue.CurrentStepIndex + 1}/{_issue.StepCount}",
                EditorStyles.boldLabel);

            EditorGUILayout.LabelField(_issue.CurrentStep.Name, new GUIStyle(GUI.skin.label) {wordWrap = true});


            _issue.CurrentStep.Draw(_issue);

            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal();

            var nextButton = "Next Step";

            if (_issue.CurrentStep.Predicate())
            {
                if (GUILayout.Button(nextButton))
                    _issue.CurrentStep.Done = true;
            }
            else
            {
                var previouslyEnabled = GUI.enabled;
                GUI.enabled = false;
                GUILayout.Button(nextButton);
                GUI.enabled = previouslyEnabled;
            }

            EditorGUILayout.EndHorizontal();
        }

        public static void End()
        {
            if (_window != null)
                _window.Close();
        }

        private void Update()
        {
            Repaint();
        }
    }
}