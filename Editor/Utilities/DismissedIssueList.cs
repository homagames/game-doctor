using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    [FilePath("ProjectSettings/DismissedIssueList.asset", FilePathAttribute.Location.ProjectFolder)]
    public class DismissedIssueList : ScriptableSingleton<DismissedIssueList>
    {
        [SerializeField]
        private List<int> DismissedHashes = new List<int>();

        public bool HasBeenDismissed([NotNull] IIssue issue)
        {
            return DismissedHashes.Contains(issue.GetHash());
        }

        public void SetDismissed([NotNull] IIssue issue, bool val = true)
        {
            if (val)
            {
                DismissedHashes.Add(issue.GetHash());
            }
            else
            {
                DismissedHashes.Remove(issue.GetHash());
            }
            
            Save(true);
        }
    }

    public static class DismissedIssues
    {
        public static bool HasBeenDismissed([NotNull] this IIssue issue)
        {
            return DismissedIssueList.instance.HasBeenDismissed(issue);
        }

        public static void SetDismissed([NotNull] this IIssue issue, bool val = true)
        {
            DismissedIssueList.instance.SetDismissed(issue, val);
        }
    }
}