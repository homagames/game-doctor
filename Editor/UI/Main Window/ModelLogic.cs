using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using HomaGames.GameDoctor.Utilities;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class GameDoctorWindow
    {
        private const string FIRST_ISSUE_FIX_FAILED_KEY = "game_doctor.first_issue_fix_failed";

        private int IssuesFailedToFixCount;

        private bool _dismissedIssuesHidden = true;

        private bool DismissedIssuesHidden
        {
            get => _dismissedIssuesHidden;
            set
            {
                if (_dismissedIssuesHidden != value)
                {
                    _dismissedIssuesHidden = value;

                    if (_dismissedIssuesHidden && GetSelectedElement() is IIssue issue)
                    {
                        if (issue.HasBeenDismissed())
                        {
                            ChangeSelectionOfDismissedIssue(issue);
                        }
                    }
                }
            }
        }
        
        private void RunAllChecksAndFix()
        {
            ListenForErrors(RunAllChecksAndFixAsync());
        }

        private async Task RunAllChecksAndFixAsync()
        {
            await RunAllChecksAsync();
            await FixAutoIssuesAsync(GetAllNonDismissedIssues().Where(issue => issue.AutomationType == AutomationType.Automatic).ToList());
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
            await RunAsync(Profile.CheckList.ToList());
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

        private void FixAutoIssues([NotNull] IReadOnlyList<IIssue> issues)
        {
            ListenForErrors(FixAutoIssuesAsync(
                issues
                    .Where(issue => 
                        issue.AutomationType == AutomationType.Automatic
                        && ! GetUiData(issue).Fixed)
                    .ToList()));
        }
        
        private async Task FixAllAutoIssuesAsync()
        {
            await FixAutoIssuesAsync(
                GetAllNonDismissedIssues()
                    .Where(issue => 
                        issue.AutomationType == AutomationType.Automatic
                        && ! GetUiData(issue).Fixed)
                    .ToList());
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
                await FixInteractiveIssueAsync(issue);
            }
        }

        private async Task FixInteractiveIssueAsync([NotNull] IIssue issue)
        {
            Rect previousPosition = position;
            if (!docked)
                position = new Rect(previousPosition) { width = 0, height = 0 };
            try
            {
                await issue.Fix().ContinueWith(task => OnInteractiveIssueFixed(task, issue, docked, previousPosition),
                    TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception e)
            {
                Debug.LogError($"Exception when fixing \"{issue.Name}\" issue:\n{e}");
            }

            CheckForIssuesFailedToFix();
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
                        await issues[i].Fix().ContinueWith(task => OnIssueFixed(task, issues[i]),
                            TaskScheduler.FromCurrentSynchronizationContext());
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Exception when fixing \"{issues[i].Name}\" issue:\n{e}");
                    }
                }

                var totalPostActions = GameDoctorFlow.PostFixActionCount;
                while (GameDoctorFlow.ExecuteNextPostFixAction())
                {
                    EditorUtility.DisplayProgressBar(scanWindowTitle,
                        $"Executing Post Action {GameDoctorFlow.PostFixActionCount}/{totalPostActions}",
                        (float) GameDoctorFlow.PostFixActionCount / totalPostActions);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                CheckForIssuesFailedToFix();
            }
        }

        private void OnInteractiveIssueFixed(
            [NotNull] Task<bool> task, [NotNull] IIssue issue,
            bool wasDocked, Rect previousPosition)
        {
            if (! wasDocked)
            {
                GetWindow<GameDoctorWindow>().Show();
                position = previousPosition;
            }
            
            OnIssueFixed(task, issue);
        }

        private void OnIssueFixed([NotNull] Task<bool> task, [NotNull] IIssue issue)
        {
            if (task.IsCompleted 
                && !task.IsFaulted
                && task.Result)
            {
                var issueUiData = GetUiData(issue);
                issueUiData.Fixed = true;
                
                OnAfterIssueFixed();
            }
            else
            {
                IssuesFailedToFixCount += 1;
            }
        }

        private void OnAfterIssueFixed()
        {
            ShowTip(TipName.IssueFixed);
        }

        private void CheckForIssuesFailedToFix()
        {
            if (IssuesFailedToFixCount > 0)
            {
                bool plural = IssuesFailedToFixCount > 1;

                string message = $"{IssuesFailedToFixCount} issue fix{(plural ? "es" : "")} failed.";
                
                if (!EditorPrefs.HasKey(FIRST_ISSUE_FIX_FAILED_KEY))
                {
                    EditorPrefs.SetBool(FIRST_ISSUE_FIX_FAILED_KEY, true);
                    message += $"\n\nThe issue{(plural ? "s" : "")} will stay in the list. Check the logs to " +
                               $"see what went wrong, or re-run the associated checks.";
                }
                
                EditorUtility.DisplayDialog("Issue Fix failed", message, "Ok");
                
                IssuesFailedToFixCount = 0;
            }
        }
        #endregion

        private static void ListenForErrors([NotNull] Task task)
        {
            task.ContinueWith(resultTask =>
            {
                if (!resultTask.IsCompleted || resultTask.IsFaulted)
                {
                    Debug.LogError($"[Game Doctor] Exception in task:{resultTask.Exception}");
                }
            });
        }
    }
}
