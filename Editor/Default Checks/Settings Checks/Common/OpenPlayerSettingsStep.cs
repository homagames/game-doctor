using System;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks.Common
{
    public class OpenPlayerSettingsStep : Step
    {
        public OpenPlayerSettingsStep() : base(IsPlayerSettingsOpen, "Go to Player Settings", Draw)
        {
        }

        private static void Draw(StepBasedIssue issue, Step step)
        {
            if (GUILayout.Button("Open Player Settings"))
            {
                SettingsService.OpenProjectSettings("Project/Player");
            }
        }

        private static bool IsPlayerSettingsOpen() => true;
    }
}