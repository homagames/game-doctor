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
            GUILayout.Label("Description");
            EditorGUILayout.EndScrollView();
        }

        [NotNull]
        private static GUIStyle TitleGuiStyle => new GUIStyle(GUI.skin.label)
        {
            fontSize = 37,
            fontStyle = FontStyle.Bold
        };

        private void Draw([NotNull] IValidationProfile profile)
        {
            SecondViewScroll = EditorGUILayout.BeginScrollView(SecondViewScroll);
            
            EditorGUILayout.Space(20);
            
            EditorGUILayout.LabelField(profile.Name, TitleGuiStyle, GetHeightOptionFor(TitleGuiStyle, profile.Name));
            
            EditorGUILayout.Space(20);

            DrawSummaryTable(profile.GetPriorityCount(), profile.GetAutomationCount());
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField($"Number of checks: {profile.CheckList.Count}");
            
            EditorGUILayout.Space(20);
            
            EditorGUILayout.SelectableLabel(profile.Description, DescriptionGUIStyle);
            
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
            
            EditorGUILayout.Space(20);
            
            EditorGUILayout.LabelField(check.Name, TitleGuiStyle, GetHeightOptionFor(TitleGuiStyle, check.Name));
            if (check.CheckResult?.Passed == true)
                EditorGUILayoutExtension.ColorLabel(new GUIContent(NBSP + "Passed", FixedWhiteTexture),
                    new Color(0.06f, 0.65f, 0.54f));

            EditorGUILayout.Space(20);

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
            
            EditorGUILayout.SelectableLabel(check.Description, DescriptionGUIStyle);
            
            EditorGUILayout.Space(30);
            
            GUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(check.CheckResult == null ||
                                               check.CheckResult.Issues.All(issue =>
                                                   issue.AutomationType != AutomationType.Automatic)))
            {
                GUILayout.Button("Fix all auto. issues", GUILayout.Width(130));
            }

            if (GUILayout.Button(check.CheckResult == null ? "Run check" : "Re-run check", GUILayout.Width(130)))
            {
                RunCheck(check);
            }
            GUILayout.EndHorizontal();
            
            EditorGUILayout.EndScrollView();
        }

        private void Draw([NotNull] IIssue issue)
        {
            var issueUiData = GetUiData(issue);
            SecondViewScroll = EditorGUILayout.BeginScrollView(SecondViewScroll);
            
            EditorGUILayout.Space(20);
            
            EditorGUILayout.LabelField(issue.Name, TitleGuiStyle, GetHeightOptionFor(TitleGuiStyle, issue.Name));

            if (issueUiData.Fixed)
            {
                EditorGUILayoutExtension.ColorLabel(new GUIContent(NBSP + "Fixed", FixedWhiteTexture), new Color(0.06f, 0.65f, 0.54f));
            }
            
            EditorGUILayout.Space(20);

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
            
            EditorGUILayout.SelectableLabel(issue.Description, DescriptionGUIStyle);
            
            EditorGUILayout.Space(10);
            
            issue.Draw();
            
            EditorGUILayout.Space(20);
            
            
            GUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(issueUiData.Fixed);
            if (GUILayout.Button("Fix", GUILayout.Width(130)))
            {
                FixIssue(issue);
            }
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Re-run check", GUILayout.Width(130)))
            {
                ICheck parentCheck =
                    Profile.CheckList.First(c =>
                        c.CheckResult != null && c.CheckResult.Issues.Contains(issue));

                RunCheck(parentCheck);
            }
            GUILayout.EndHorizontal();
            
            EditorGUILayout.EndScrollView();
        }

        private static GUILayoutOption GetHeightOptionFor(GUIStyle style, string content)
        {
            return GUILayout.Height(style.CalcSize(new GUIContent(content)).y);
        }
    }
}
