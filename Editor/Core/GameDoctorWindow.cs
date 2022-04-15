using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor
{
    public class GameDoctorWindow : EditorWindow
    {
        private ValidationProfileConfiguration profile;
        private Dictionary<Check, CheckInstance> resultCache = new Dictionary<Check, CheckInstance>();

        [MenuItem("Window/Homa Games/Game Doctor")]
        static void Init()
        {
            GameDoctorWindow window = (GameDoctorWindow) EditorWindow.GetWindow(typeof(GameDoctorWindow));
            window.titleContent = new GUIContent("Game Doctor");
            window.Show();
        }

        void OnGUI()
        {
            profile = EditorGUILayout.ObjectField("Validation Profile : ", profile,
                typeof(ValidationProfileConfiguration),
                false) as ValidationProfileConfiguration;

            if (profile)
                foreach (var check in profile.Checks)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(check.Name);
                    if (GUILayout.Button("Fix"))
                    {
                        resultCache[check] = check.Execute();
                    }

                    if (resultCache.TryGetValue(check, out CheckInstance result))
                    {
                        if (result.Progress < 1)
                        {
                            EditorGUILayout.LabelField($"{result.Progress * 100}%");
                        }
                        else if (result.Passed)
                            EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_Valid"));
                        else
                            EditorGUILayout.LabelField(EditorGUIUtility.IconContent("d_Invalid"));
                    }

                    EditorGUILayout.EndHorizontal();
                }
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }
}