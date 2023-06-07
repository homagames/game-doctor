using System.Collections.Generic;
using UnityEditor;

namespace HomaGames.GameDoctor.Ui
{
    public partial class GameDoctorWindow
    {
        private enum TipName
        {
            IssueFixed,
            IssueDismissed
        }

        private static void ShowTip(TipName tipName)
        {
            Tip tip = GetTipByName[tipName];
            string editorPrefKey = $"game_doctor.{tip.EditorPrefKeySuffix}";
            
            if (!EditorPrefs.HasKey(editorPrefKey))
            {
                EditorPrefs.SetBool(editorPrefKey, true);

                EditorUtility.DisplayDialog("Tip", tip.DialogText, "Understood!");
            }
        }

        private static readonly Dictionary<TipName, Tip> GetTipByName = new Dictionary<TipName, Tip>
        {
            [TipName.IssueFixed] = new Tip
            {
                EditorPrefKeySuffix = "first_issue_fixed",
                DialogText = "Once issues are supposedly fixed, they will appear " +
                             "as \"fixed\" in the navigation tree. To make sure they were fixed correctly, " +
                             "run the check again."
            },
            [TipName.IssueDismissed] = new Tip
            {
                EditorPrefKeySuffix = "first_issue_dismissed",
                DialogText = "You just dismissed an issue. This specific issue won't be displayed anymore. " +
                             "You can make it appear again by toggling \"Show dismissed\" on."
            },
        };

        private struct Tip
        {
            public string EditorPrefKeySuffix;
            public string DialogText;
        }
    }
}