using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;

namespace HomaGames.GameDoctor.Core
{
    [InitializeOnLoad]
    public static class DismissedIssues
    {
        private const string SettingsFilePath = "ProjectSettings/GameDoctorDismissed.txt";
        
        private static readonly HashSet<int> DismissedHash = new HashSet<int>();

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
        
        public static bool HasBeenDismissed([NotNull] this IIssue issue)
        {
            return DismissedHash.Contains(issue.GetHash());
        }

        public static void SetDismissed([NotNull] this IIssue issue, bool val = true)
        {
            if (val)
            {
                DismissedHash.Add(issue.GetHash());
            }
            else
            {
                DismissedHash.Remove(issue.GetHash());
            }
            
            UpdateSettingsFile();
        }

        private static void UpdateSettingsFile()
        {
            File.WriteAllLines(SettingsFilePath, DismissedHash.Select(hash => hash.ToString(CultureInfo.InvariantCulture)));
        }
    }
}