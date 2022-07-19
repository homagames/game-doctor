using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    [InitializeOnLoad]
    public static class DismissedIssues
    {
        private const string SettingsFilePath = "";
        
        private static readonly List<int> DismissedHash = new List<int>();

        static DismissedIssues()
        {
            if (File.Exists(SettingsFilePath))
            {
                foreach (var line in File.ReadAllLines(SettingsFilePath))
                {
                    if (int.TryParse(line, out var hash))
                        DismissedHash.Add(hash);
                }
            }
        }
        
        public static bool HasBeenDismissed(this IIssue issue)
        {
            return DismissedHash.Contains(issue.GetHash());
        }

        public static void SetDismissed(this IIssue issue, bool val = true)
        {
            DismissedHash.Add(issue.GetHash());
            UpdateSettingsFile();
        }

        private static void UpdateSettingsFile()
        {
            File.WriteAllLines(SettingsFilePath, DismissedHash.Select(hash => hash.ToString(CultureInfo.InvariantCulture)));
        }
    }
}