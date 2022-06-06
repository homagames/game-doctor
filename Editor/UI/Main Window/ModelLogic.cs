using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class GameDoctorWindow
    {
        private const string FIRST_FIXED_ISSUE_KEY = "game_doctor.first_issue_fixed";
        
        private void RunAllChecksAndFix()
        {
            ListenForErrors(RunAllChecksAndFixAsync());
        }

        private async Task RunAllChecksAndFixAsync()
        {
            await RunAllChecksAsync();
            await FixAutoIssuesAsync(GetAllIssues().Where(issue => issue.AutomationType == AutomationType.Automatic).ToList());
        }

        #region Checks
        private void RunAllChecks()
        {
            ListenForErrors(RunAllChecksAsync());
        }

        private void RunCheck([NotNull] ICheck check)
        {
            ListenForErrors(RunCheckAsync(check));
        }
        
        private async Task RunAllChecksAsync()
        {
            await RunAsync(Profile.CheckList);
        }
        
        private async Task RunCheckAsync([NotNull] ICheck check)
        {
            await RunAsync(new List<ICheck> { check });
        }
        
        private async Task RunAsync([NotNull] IReadOnlyList<ICheck> checks)
        {
            float maxI = checks.Count;
            string scanWindowTitle = "Executing all checks";
            string scanWindowContent = "Looking for issues in the project";

            try
            {
                for (int i = 0; i < maxI; i++)
                {
                    EditorUtility.DisplayProgressBar(scanWindowTitle, scanWindowContent, i / maxI);
                    try
                    {
                        await checks[i].Execute();
                        if (checks[i].CheckResult != null)
                        {
                            foreach (var issue in checks[i].CheckResult.Issues)
                            {
                                GetUiData(issue).Fixed = false;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Exception when running \"{checks[i].Name}\" check:\n{e}");
                    }
                }
            }
            finally
            {
                OnIssueListChanged();
                EditorUtility.ClearProgressBar();
            }
        }
        #endregion

        #region Issues
        private void FixAllAutoIssues()
        {
            ListenForErrors(FixAllAutoIssuesAsync());
        }

        private async Task FixAllAutoIssuesAsync()
        {
            await FixAutoIssuesAsync(GetAllIssues().Where(issue => issue.AutomationType == AutomationType.Automatic).ToList());
        }
        
        private void FixIssue([NotNull] IIssue issue)
        {
            ListenForErrors(FixIssueAsync(issue));
        }

        private async Task FixIssueAsync([NotNull] IIssue issue)
        {
            if (issue.AutomationType == AutomationType.Automatic)
            {
                await FixAutoIssuesAsync(new List<IIssue> { issue });
            }
            else
            {
                await issue.Fix().ContinueWith(task => OnIssueFixed(task, issue));
            }

            OnAfterIssueFixed();
        }
        
        private async Task FixAutoIssuesAsync([NotNull] IReadOnlyList<IIssue> issues)
        {
            float maxI = issues.Count;
            string scanWindowTitle = "Fixing issues";

            try
            {
                for (int i = 0; i < maxI; i++)
                {
                    EditorUtility.DisplayProgressBar(scanWindowTitle, $"Fixing \"{issues[i].Name}", i / maxI);
                    try
                    {
                        await issues[i].Fix().ContinueWith(task => OnIssueFixed(task, issues[i]));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Exception when fixing \"{issues[i].Name}\" issue:\n{e}");
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                OnAfterIssueFixed();
            }
        }

        private void OnIssueFixed([NotNull] Task task, [NotNull] IIssue issue)
        {
            if (task.IsCompletedSuccessfully)
            {
                var issueUiData = GetUiData(issue);
                issueUiData.Fixed = true;
            }
        }

        private void OnAfterIssueFixed()
        {
            if (!EditorPrefs.HasKey(FIRST_FIXED_ISSUE_KEY))
            {
                EditorPrefs.SetBool(FIRST_FIXED_ISSUE_KEY, true);

                EditorUtility.DisplayDialog("Tip", "Once issues are supposedly fixed, they will appear " +
                                                   "as \"fixed\" in the navigation tree. To make sure they were fixed correctly, " +
                                                   "run the check again.", "Understood!");
            }
        }
        #endregion


        private static void ListenForErrors([NotNull] Task task)
        {
            task.ContinueWith(resultTask =>
            {
                if (!resultTask.IsCompletedSuccessfully)
                {
                    Debug.LogError($"[Game Doctor] Exception in task:{resultTask.Exception}");
                }
            });
        }
    }
}
