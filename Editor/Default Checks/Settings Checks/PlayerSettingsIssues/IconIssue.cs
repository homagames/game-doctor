using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Checks.Common;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEditor.Android;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class IconIssue : StepBasedIssue
    {
        public IconIssue() : base(new List<Step>()
        {
            new OpenPlayerSettingsStep(),
            new Step(() => !HasIssue, "Assign a texture.", DrawStep)
        }, "Icon Issue", "You should assign an Icon for your game.")
        {
        }

        private static void DrawStep(StepBasedIssue issue, Step step)
        {
            EditorGUILayout.LabelField("Assign a texture to your game in the Icon section of the Player Settings.",
                EditorStyles.wordWrappedLabel);
            var icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
            var newIcon = EditorGUILayout.ObjectField("Icon", icons[0], typeof(Texture2D), false) as Texture2D;
            icons[0] = newIcon;
            PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, icons);
        }

        public static bool HasIssue
        {
            get
            {
                var icons = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown);
                return !(icons.Length > 0) || !icons[0];
            }
        }
    }
}