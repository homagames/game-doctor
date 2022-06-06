using System.Collections.Generic;
using System.Linq;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class GameDoctorWindow
    {
        private const int FooterPadding = 10;

        private void DrawFooter(float footerSize)
        {
            int passedCheckCount = 0, autoFixableCheckCount = 0, failedCheckCount = 0;
            var checks = Profile.CheckList;
            passedCheckCount = ReadPassedCheckCount(checks, ref passedCheckCount, ref autoFixableCheckCount, ref failedCheckCount);
            
            var priorityCount = Profile.GetPriorityCount();
            var highPriorityIssueCount = priorityCount.High;
            var mediumPriorityIssueCount = priorityCount.Medium;
            var lowPriorityIssueCount = priorityCount.Low;

            if (passedCheckCount + autoFixableCheckCount + failedCheckCount == 0)
            {
                EditorGUILayout.LabelField("Run a scan to detect issues in the project...");
                return;
            }


            Rect footerRect = EditorGUILayout.GetControlRect(
                false, 
                footerSize - FooterPadding*2, 
                new GUIStyle
                {
                    margin = new RectOffset(FooterPadding, FooterPadding, FooterPadding, FooterPadding)
                });

            Rect firstPieChartRect = new Rect(footerRect)
            {
                width = footerRect.width / 2,
            };
            Rect secondPieChartRect = new Rect(footerRect)
            {
                x = footerRect.width / 2,
                width = footerRect.width / 2,
            };
        
            EditorGUIExtension.DrawPieChart(firstPieChartRect, 
            
                new EditorGUIExtension.PieChartValue
                {
                    Value = passedCheckCount,
                    Color = new Color(0f, 0.45f, 0.7f),
                    Label = "Passed Checks"
                },
                new EditorGUIExtension.PieChartValue
                {
                    Value = autoFixableCheckCount,
                    Color = new Color(0f, 0.62f, 0.45f),
                    Label = "Auto-fixable checks"
                },
                new EditorGUIExtension.PieChartValue
                {
                    Value = failedCheckCount,
                    Color = new Color(0.84f, 0.37f, 0f),
                    Label = "Failed checks"
                });
        
            if (highPriorityIssueCount + mediumPriorityIssueCount + lowPriorityIssueCount > 0)
            {
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
            else
            {
                GUI.Label(secondPieChartRect, "No issue found, well done!");
            }
        }

        private static int ReadPassedCheckCount(IEnumerable<ICheck> checks, ref int passedCheckCount, ref int autoFixableCheckCount,
            ref int failedCheckCount)
        {
            foreach (var check in checks)
            {
                if (check.CheckResult == null)
                    continue;

                if (check.CheckResult.Passed)
                {
                    passedCheckCount += 1;
                    continue;
                }

                bool isAutoFixable = check.CheckResult.Issues.All(issue => issue.AutomationType == AutomationType.Automatic);

                if (isAutoFixable)
                    autoFixableCheckCount += 1;
                else
                    failedCheckCount += 1;
            }

            return passedCheckCount;
        }
    }
}
