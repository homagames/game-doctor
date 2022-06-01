using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class SplitViewWindow
    {
    
        private void DrawFooter(float footerSize)
        {
            int passedCheckCount = 0, autoFixableCheckCount = 0, failedCheckCount = 0;
            int highPriorityIssueCount = 0, mediumPriorityIssueCount = 0, lowPriorityIssueCount = 0;
        
            var checks = Profile.CheckList;
            foreach (var check in checks)
            {
                if (check.CheckResult == null)
                    continue;

                if (check.CheckResult.Passed)
                {
                    passedCheckCount += 1;
                    continue;
                }

                bool isAutoFixable = true;
                foreach (var issue in check.CheckResult.Issues)
                {
                    if (issue.AutomationType != AutomationType.Automatic)
                        isAutoFixable = false;

                    switch (issue.Priority)
                    {
                        default:
                        case Priority.Low:
                            lowPriorityIssueCount += 1;
                            break;
                        case Priority.Medium:
                            mediumPriorityIssueCount += 1;
                            break;
                        case Priority.High:
                            highPriorityIssueCount += 1;
                            break;
                    }
                }

                if (isAutoFixable)
                    autoFixableCheckCount += 1;
                else
                    failedCheckCount += 1;
            }

            if (passedCheckCount + autoFixableCheckCount + failedCheckCount == 0)
            {
                EditorGUILayout.LabelField("Run a scan to detect issues in the project...");
                return;
            }

        
            int footerPadding = 10;
            Rect footerRect = EditorGUILayout.GetControlRect(false, footerSize - footerPadding*2, new GUIStyle() {margin = new RectOffset(footerPadding, footerPadding, footerPadding, footerPadding)});

            Rect firstPieChartRect = new Rect(
                footerRect.x, footerRect.y,
                footerRect.width / 2, footerRect.height);
            Rect secondPieChartRect = new Rect(
                footerRect.width / 2 + footerRect.x, footerRect.y,
                footerRect.width / 2, footerRect.height);
        
            EditorGUIExtension.DrawPieChart(firstPieChartRect, 
            
                new EditorGUIExtension.PieChartValue
                {
                    Value = passedCheckCount,
                    Color = new Color(0.18f, 0.88f, 0.85f),
                    Label = "Passed Checks"
                },
                new EditorGUIExtension.PieChartValue
                {
                    Value = autoFixableCheckCount,
                    Color = Color.yellow,
                    Label = "Auto-fixable checks"
                },
                new EditorGUIExtension.PieChartValue
                {
                    Value = failedCheckCount,
                    Color = Color.magenta,
                    Label = "Failed checks"
                });
        
            EditorGUIExtension.DrawPieChart(secondPieChartRect, 
                new EditorGUIExtension.PieChartValue
                {
                    Value = highPriorityIssueCount,
                    Color = HighPriorityColor,
                    Label = "High priority"
                },
                new EditorGUIExtension.PieChartValue
                {
                    Value = mediumPriorityIssueCount,
                    Color = MediumPriorityColor,
                    Label = "Medium priority"
                },
                new EditorGUIExtension.PieChartValue
                {
                    Value = lowPriorityIssueCount,
                    Color = LowPriorityColor,
                    Label = "Low priority"
                });
        }
    }
}