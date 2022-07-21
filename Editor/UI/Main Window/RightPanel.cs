using System.Collections.Generic;
using System.Linq;
using HomaGames.GameDoctor.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class GameDoctorWindow
    {
        private Vector2 SecondViewScroll;
        
        private GUIStyle DescriptionGUIStyle => new GUIStyle(GUI.skin.label) {wordWrap = true, richText = true};

        private void DrawRightPanel()
        {
            object selectedObject = GetSelectedElement();

            switch (selectedObject)
            {
                case null:
                    DrawPlaceHolder();
                    break;
                case IValidationProfile selectedProfile:
                    Draw(selectedProfile);
                    break;
                case ICheck selectedCheck:
                    Draw(selectedCheck);
                    break;
                case IIssue selectedIssue:
                    Draw(selectedIssue);
                    break;
            }
        }

        private void DrawPlaceHolder()
        {
            SecondViewScroll = EditorGUILayout.BeginScrollView(SecondViewScroll);
            EditorGUILayout.EndScrollView();
        }

        [NotNull]
        private static GUIStyle TitleGuiStyle => new GUIStyle(GUI.skin.label)
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold
        };

        private void Draw([NotNull] IValidationProfile profile)
        {
            SecondViewScroll = EditorGUILayout.BeginScrollView(SecondViewScroll);
            
            EditorGUILayout.Space(10);
            
            EditorGUI.LabelField(GetRectFor(TitleGuiStyle, profile.Name), profile.Name, TitleGuiStyle);
            
            EditorGUILayout.Space(10);

            DrawSummaryTable(profile.GetPriorityCount(), profile.GetAutomationCount());
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField($"Number of checks: {profile.CheckList.Count}");
            
            EditorGUILayout.Space(20);
            
            EditorGUI.SelectableLabel(GetRectFor(DescriptionGUIStyle, profile.Description), profile.Description, DescriptionGUIStyle);
            
            EditorGUILayout.EndScrollView();
        }
        
        [NotNull]
        private static GUIStyle TableGuiStyle => new GUIStyle(GUI.skin.label)
        {
            imagePosition = ImagePosition.ImageLeft
        };

        private void DrawSummaryTable(PriorityCount priorities, AutomationCount automations)
        {
            void DrawCell(int value, Texture texture2D)
            {
                EditorGUILayout.LabelField(new GUIContent("   " + value, texture2D), TableGuiStyle,
                    GUILayout.Width(100));
            }

            GUILayout.BeginHorizontal();
            DrawCell(priorities.High, HighPriorityTexture);
            DrawCell(automations.Automatic, AutomaticTexture);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            DrawCell(priorities.Medium, MediumPriorityTexture);
            DrawCell(automations.Interactive, InteractiveTexture);
            GUILayout.EndHorizontal();
            
            DrawCell(priorities.Low, LowPriorityTexture);
        }

        private void Draw([NotNull] ICheck check)
        {
            SecondViewScroll = EditorGUILayout.BeginScrollView(SecondViewScroll);
            
            EditorGUILayout.Space(10);
            
            EditorGUI.LabelField(GetRectFor(TitleGuiStyle, check.Name), check.Name, TitleGuiStyle);
            if (check.CheckResult?.Passed == true)
                EditorGUILayoutExtension.ColorLabel(new GUIContent(NBSP + "Passed", FixedWhiteTexture),
                    new Color(0.06f, 0.65f, 0.54f));

            EditorGUILayout.Space(10);

            DrawSummaryTable(check.GetPriorityCount(), check.GetAutomationCount());
            
            if (check.Importance == ImportanceType.Mandatory)
            {
                EditorGUILayout.Space(10);
                
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("This check is", GUILayout.Width(75));
                EditorGUILayout.LabelField(new GUIContent("Important", MandatoryTexture), GUILayout.ExpandWidth(false));
                GUILayout.EndHorizontal();
            }
            
            EditorGUILayout.Space(20);
            
            EditorGUI.SelectableLabel(GetRectFor(DescriptionGUIStyle, check.Description), check.Description, DescriptionGUIStyle);

            EditorGUILayout.EndScrollView();
            
            
            GUILayout.BeginHorizontal(new GUIStyle() { padding = new RectOffset(5, 5, 5, 5) });
            using (new EditorGUI.DisabledScope(check.CheckResult == null ||
                                               check.CheckResult.Issues.All(issue =>
                                                   issue.AutomationType != AutomationType.Automatic)))
            {
                if (GUILayout.Button("Fix all auto. issues", GUILayout.Width(130)))
                {
                    FixAutoIssues(check.CheckResult!.Issues);
                }
            }

            if (GUILayout.Button(check.CheckResult == null ? "Run check" : "Re-run check", GUILayout.Width(130)))
            {
                RunCheck(check);
            }
            GUILayout.EndHorizontal();
        }

        private void Draw([NotNull] IIssue issue)
        {
            var issueUiData = GetUiData(issue);
            SecondViewScroll = EditorGUILayout.BeginScrollView(SecondViewScroll);
            
            EditorGUILayout.Space(10);
            
            EditorGUI.LabelField(GetRectFor(TitleGuiStyle, issue.Name), issue.Name, TitleGuiStyle);

            if (issueUiData.Fixed)
            {
                EditorGUILayoutExtension.ColorLabel(new GUIContent(NBSP + "Fixed", FixedWhiteTexture), new Color(0.06f, 0.65f, 0.54f));
            }
            if (issue.HasBeenDismissed())
            {
                EditorGUILayout.LabelField(new GUIContent(NBSP + "Dismissed", DismissedTexture));
            }
            
            EditorGUILayout.Space(10);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Priority:", GUILayout.Width(45));
            Color priorityColorLabel = GetColorFor(issue.Priority) * 0.5f + GUI.color * 0.5f;
            EditorGUILayoutExtension.ColorLabel(GetColorableGuiContentFor(issue.Priority), priorityColorLabel, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Fix type:", GUILayout.Width(50));
            EditorGUILayout.LabelField(GetGuiContentFor(issue.AutomationType), GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            EditorGUILayout.Space(20);
            
            EditorGUI.SelectableLabel(GetRectFor(DescriptionGUIStyle, issue.Description), issue.Description, DescriptionGUIStyle);
            
            EditorGUILayout.Space(10);
            
            issue.Draw();

            EditorGUILayout.EndScrollView();
            
            
            GUILayout.BeginHorizontal(new GUIStyle() { padding = new RectOffset(5, 5, 5, 5) });

            EditorGUI.BeginDisabledGroup(issueUiData.Fixed);
            if (GUILayout.Button("Fix", GUILayout.Width(130)))
            {
                FixIssue(issue);
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Re-run check", GUILayout.Width(130)))
            {
                RunCheck(GetParentCheck(issue));
            }

            bool isDismissed = issue.HasBeenDismissed();
            if (GUILayout.Button(isDismissed ? "Un-dismiss" : "Dismiss", GUILayout.Width(130)))
            {
                ShowTip(TipName.IssueDismissed);
                
                issue.SetDismissed(! isDismissed);
                
                if (DismissedIssuesHidden && issue.HasBeenDismissed())
                {
                    ChangeSelectionOfDismissedIssue(issue);
                }
            }
            GUILayout.EndHorizontal();
        }

        [NotNull]
        private ICheck GetParentCheck([NotNull] IIssue issue)
        {
            return Profile.CheckList.First(c =>
                    c.CheckResult != null && c.CheckResult.Issues.Contains(issue));
        }

        private static Rect GetRectFor([NotNull] GUIStyle style, string content)
        {
            return GUILayoutUtility.GetRect(new GUIContent(content), style);
        }
    }
}
