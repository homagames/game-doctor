using System;
using System.Collections.Generic;
using System.Linq;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class SplitViewWindow
    {
        private Vector2 FirstViewScroll;
        private float ScrollbarSize;
        
        private bool HideFixed;

        private string _searchString = string.Empty;
        private string[] SearchWords;
        private string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value ?? string.Empty;
                SearchWords = _searchString?.Split(" ");
            }
        }

        private LeftPanelSortBy SortOrder = LeftPanelSortBy.Default;
        private enum LeftPanelSortBy
        {
            Default, Mandatory, Name, HighPriority, MediumPriority, LowPriority
        }

        private void DrawLeftPanel()
        {
            EditorGUILayoutExtension.BeginToolBar();
            SearchString = EditorGUILayoutExtension.ToolBarSearchBar(SearchString, GUILayout.ExpandWidth(true));
            HideFixed = EditorGUILayoutExtension.ToolBarToggle(HideFixed, "Hide fixed");
            EditorGUILayoutExtension.EndToolBar();

            EditorGUILayoutExtension.BeginToolBar();
            bool sortOrderSet = false;
            
            void SetSortOrder(LeftPanelSortBy newSortOrder)
            {
                SortOrder = newSortOrder;
                sortOrderSet = true;
            }
            
            if (EditorGUILayoutExtension.ToolBarToggle(SortOrder == LeftPanelSortBy.Mandatory, new GUIContent(MandatoryTexture), GUILayout.MaxWidth(30)))
                SetSortOrder(LeftPanelSortBy.Mandatory);
            if (EditorGUILayoutExtension.ToolBarToggle(SortOrder == LeftPanelSortBy.Name, "Name", GUILayout.ExpandWidth(true)))
                SetSortOrder(LeftPanelSortBy.Name);
            if (EditorGUILayoutExtension.ToolBarToggle(SortOrder == LeftPanelSortBy.HighPriority, new GUIContent(HighPriorityTexture), GUILayout.Width(TotalNodeSize)))
                SetSortOrder(LeftPanelSortBy.HighPriority);
            if (EditorGUILayoutExtension.ToolBarToggle(SortOrder == LeftPanelSortBy.MediumPriority, new GUIContent(MediumPriorityTexture), GUILayout.Width(TotalNodeSize)))
                SetSortOrder(LeftPanelSortBy.MediumPriority);
            if (EditorGUILayoutExtension.ToolBarToggle(SortOrder == LeftPanelSortBy.LowPriority, new GUIContent(LowPriorityTexture), GUILayout.Width(TotalNodeSize)))
                SetSortOrder(LeftPanelSortBy.LowPriority);

            if (!sortOrderSet)
                SortOrder = LeftPanelSortBy.Default;
            
            EditorGUILayoutExtension.ToolBarSpace(ScrollbarSize);
            EditorGUILayoutExtension.EndToolBar();

            float scrollViewWidth = GetCurrentLayoutWidth();
            FirstViewScroll = EditorGUILayout.BeginScrollView(FirstViewScroll);
            UpdateScrollViewWidth(scrollViewWidth);
            DrawNode(Profile);
            EditorGUILayout.EndScrollView();
        }

        private void DrawNode(IValidationProfile profile)
        {
            ProfileUiData uiData = GetUiData(profile);
            GUIContent nodeContent = new GUIContent(" " /* NBSP */ + profile.Name, ProfileTexture);
            DrawFoldoutTreeElement(uiData, nodeContent, profile.GetPriorityCount(), () =>
            {
                foreach (var check in Filter(SortForLeftPanel(profile.CheckList)))
                {
                    DrawNode(check);
                }
            });
        }

        private void DrawNode(ICheck check)
        {
            CheckUiData uiData = GetUiData(check);

            var containsIssues = check.CheckResult?.Passed != true;
            GUIContent nodeContent = new GUIContent(
                " " /* NBSP */ + check.Name, 
                !containsIssues ? FixedColoredTexture : CheckTexture);
            DrawFoldoutTreeElement(uiData, nodeContent, check.GetPriorityCount(), () =>
            {

                if (check.CheckResult != null)
                {
                    foreach (var issue in Filter(SortForLeftPanel(check.CheckResult.Issues)))
                    {
                        DrawNode(issue);
                    }
                }
            }, containsIssues, containsIssues && check.Importance == ImportanceType.Mandatory);
        }

        private void DrawFoldoutTreeElement(BaseFoldoutUiData uiData, GUIContent nodeContent, PriorityCount priorityCount,
            Action drawInside, bool drawAsFoldout = true, bool drawMandatory = false)
        {
            DrawNodeBefore(uiData);
            if (drawAsFoldout)
                uiData.Expanded.target = EditorGUILayout.Foldout(uiData.Expanded.target, nodeContent);
            else
                EditorGUILayout.LabelField(nodeContent);
            
            if (drawMandatory)
            {
                var lastRect = GUILayoutUtility.GetLastRect();
                Rect mandatoryIconRect = new Rect(lastRect) {width = 20, x = lastRect.x + 2};
                GUI.DrawTexture(mandatoryIconRect, MandatoryTexture);
            }

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            Rect priorityRect =
                EditorGUILayout.GetControlRect(
                    GUILayout.Width(TotalNodeSize * 3 - EditorGUIUtility.standardVerticalSpacing));
            priorityRect.width += EditorGUIUtility.standardVerticalSpacing;

            float heightDifference = TotalNodeSize - priorityRect.height;
            priorityRect.height += heightDifference;
            priorityRect.y -= heightDifference / 2;
            DrawPriorities(priorityRect, priorityCount);
            EditorGUI.indentLevel = indent;
            DrawNodeAfter(uiData);

            if (EditorGUILayout.BeginFadeGroup(uiData.Expanded.faded))
            {
                EditorGUI.indentLevel += 2;
                drawInside.Invoke();
                EditorGUI.indentLevel -= 2;
            }

            EditorGUILayout.EndFadeGroup();
        }

        private void DrawPriorities(Rect priorityRect, PriorityCount priorityCount)
        {
            Color WithAlpha(Color color, float alpha)
            {
                return new Color(color.r, color.g, color.b, alpha);
            }

            var rectWidth = priorityRect.width / 3;
            Rect rect1 = new Rect(priorityRect) {width = rectWidth};
            Rect rect2 = new Rect(priorityRect) {width = rectWidth, x = priorityRect.x + rectWidth};
            Rect rect3 = new Rect(priorityRect) {width = rectWidth, x = priorityRect.x + 2 * rectWidth};

            var backgroundAlpha = 0.1f;
            EditorGUI.DrawRect(rect1, WithAlpha(HighPriorityColor, backgroundAlpha));
            EditorGUI.DrawRect(rect2, WithAlpha(MediumPriorityColor, backgroundAlpha));
            EditorGUI.DrawRect(rect3, WithAlpha(LowPriorityColor, backgroundAlpha));

            EditorGUIExtension.DrawVerticalSeparator(rect1.position, rect1.height, 1);
            EditorGUIExtension.DrawVerticalSeparator(rect2.position, rect2.height, 1);
            EditorGUIExtension.DrawVerticalSeparator(rect3.position, rect3.height, 1);

            GUIStyle labelStyle = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter};
            EditorGUI.LabelField(rect1, priorityCount.High.ToString(), labelStyle);
            EditorGUI.LabelField(rect2, priorityCount.Medium.ToString(), labelStyle);
            EditorGUI.LabelField(rect3, priorityCount.Low.ToString(), labelStyle);
        }

        private void DrawNode(IIssue issue)
        {
            IssueUiData uiData = GetUiData(issue);
            if (!HideFixed || !uiData.Fixed)
            {
                DrawNodeBefore(uiData);
                
                GUIContent issueContent = new GUIContent(
                    " " /* NBSP */ + issue.Name, 
                    uiData.Fixed ? FixedColoredTexture : GetTextureFor(issue.AutomationType));

                EditorGUILayout.LabelField(issueContent);

                var priorityIconGUIStyle = new GUIStyle(GUIStyle.none) { margin = new RectOffset(0, 7, 0, 0)};
                GUI.DrawTexture(
                    EditorGUILayout.GetControlRect(
                        false, EditorGUIUtility.singleLineHeight, 
                        priorityIconGUIStyle, GUILayout.Width(16)), 
                    GetTextureFor(issue.Priority));
                
                DrawNodeAfter(uiData);
            }
        }

        private static GUIStyle SelectedNodeStyle => new GUIStyle("OL SelectedRow")
            {padding = new RectOffset(0, 0, UpperNodeMargin, LowerNodeMargin), margin = new RectOffset()};

        private static GUIStyle RegularNodeStyle => new GUIStyle()
            {padding = new RectOffset(0, 0, UpperNodeMargin, LowerNodeMargin), margin = new RectOffset()};

        private static void DrawNodeBefore(BaseUiData uiData)
        {
            EditorGUILayout.BeginHorizontal(uiData.Selected ? SelectedNodeStyle : RegularNodeStyle,
                GUILayout.ExpandWidth(true));
            EditorGUI.indentLevel += 1;
        }

        private static void DrawNodeAfter(BaseUiData uiData)
        {
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.EndHorizontal();
            Rect nodeRect = GUILayoutUtility.GetLastRect();
            EditorGUILayoutExtension.DrawHorizontalSeparator(1);


            if (Event.current.type == EventType.MouseDown && nodeRect.Contains(Event.current.mousePosition))
            {
                uiData.Selected = true;
                GUI.changed = true;
            }
        }
        
        // This method must be called in the left side scroll view, to update the scroll bar width
        private void UpdateScrollViewWidth(float totalScrollViewWidth)
        {
            float scrollViewInsideWidth = GetCurrentLayoutWidth();
            if (Event.current.type == EventType.Repaint)
            {
                ScrollbarSize = totalScrollViewWidth - scrollViewInsideWidth;
            }
        }

        #region Sorting
        private IEnumerable<ICheck> SortForLeftPanel(IEnumerable<ICheck> checkList)
        {
            List<ICheck> output = new List<ICheck>(checkList);

            int CountPriority(ICheck check, Priority priority) =>
                check.CheckResult?.Issues.Count(issue => issue.Priority == priority) ?? 0;

            int ComparePriority(ICheck c1, ICheck c2, Priority priority) =>
                CountPriority(c2, priority) - CountPriority(c1, priority);

            int DefaultCompare(ICheck c1, ICheck c2)
            {
                if (c1.Importance != c2.Importance)
                {
                    if (c1.Importance == ImportanceType.Mandatory) return -1;
                    if (c2.Importance == ImportanceType.Mandatory) return 1;
                }
                return string.Compare(c1.Name, c2.Name, StringComparison.Ordinal);
            } 

            switch (SortOrder)
            {
                default:
                case LeftPanelSortBy.Name:
                    output.Sort((c1, c2) => string.Compare(c1.Name, c2.Name, StringComparison.Ordinal));
                    break;
                case LeftPanelSortBy.Mandatory:
                    output.Sort(DefaultCompare);
                    break;
                case LeftPanelSortBy.HighPriority:
                    output.Sort((c1, c2) =>
                    {
                        int value = ComparePriority(c1, c2, Priority.High);
                        if (value != 0) return value;
                        return DefaultCompare(c1, c2);
                    });
                    break;
                case LeftPanelSortBy.MediumPriority:
                    output.Sort((c1, c2) =>
                    {
                        int value = ComparePriority(c1, c2, Priority.Medium);
                        if (value != 0) return value;
                        return DefaultCompare(c1, c2);
                    });
                    break;
                case LeftPanelSortBy.LowPriority:
                    output.Sort((c1, c2) =>
                    {
                        int value = ComparePriority(c1, c2, Priority.Low);
                        if (value != 0) return value;
                        return DefaultCompare(c1, c2);
                    });
                    break;
            }
            
            output.Sort((c1, c2) =>
            {
                if ((c1.CheckResult?.Passed ?? false) == (c2.CheckResult?.Passed ?? false)) return 0;
                if (c1.CheckResult?.Passed ?? false) return 1;
                return -1;
            });

            return output;
        }
        
        private IEnumerable<IIssue> SortForLeftPanel(IEnumerable<IIssue> issueList)
        {
            List<IIssue> output = new List<IIssue>(issueList);

            int ComparePriority(IIssue i1, IIssue i2, Priority priority)
            {
                if (i1.Priority != i2.Priority)
                {
                    if (i1.Priority == priority) return -1;
                    if (i2.Priority == priority) return 1;
                }
                return DefaultCompare(i1, i2);
            }

            int DefaultCompare(IIssue i1, IIssue i2)
            {
                if (i1.Priority == i2.Priority)
                    return string.Compare(i1.Name, i2.Name, StringComparison.Ordinal);

                return i1.Priority.CompareTo(i2.Priority);
            }

            switch (SortOrder)
            {
                case LeftPanelSortBy.Name:
                    output.Sort((i1, i2) => string.Compare(i1.Name, i2.Name, StringComparison.Ordinal));
                    break;
                default:
                case LeftPanelSortBy.Mandatory:
                    output.Sort(DefaultCompare);
                    break;
                case LeftPanelSortBy.HighPriority:
                    output.Sort((i1, i2) => ComparePriority(i1, i2, Priority.High));
                    break;
                case LeftPanelSortBy.MediumPriority:
                    output.Sort((i1, i2) => ComparePriority(i1, i2, Priority.Medium));
                    break;
                case LeftPanelSortBy.LowPriority:
                    output.Sort((i1, i2) => ComparePriority(i1, i2, Priority.Low));
                    break;
            }

            return output;
        }
        #endregion

        #region Filtering
        private const float RELEVANCE_RATIO_MINIMUM = 0.7f;
        
        private IEnumerable<ICheck> Filter(IEnumerable<ICheck> checkList)
        {
            return checkList.Where(check =>
            {
                if (HideFixed && check.CheckResult?.Passed == true)
                    return false;
                    
                if (!string.IsNullOrWhiteSpace(SearchString))
                {
                    if (! MatchSearchString(check.Name))
                    {
                        if (check.CheckResult == null
                            || !Filter(check.CheckResult.Issues).Any())
                            return false;
                    }
                }
                
                return true;
            });
        }
        
        private IEnumerable<IIssue> Filter(IEnumerable<IIssue> issueList)
        {
            return issueList.Where(issue =>
            {
                if (!string.IsNullOrWhiteSpace(SearchString))
                {
                    if (! MatchSearchString(issue.Name))
                    {
                        return false;
                    }
                }
                
                return true;
            });
        }

        private bool MatchSearchString(string input)
        {
            int relevance = ComputeRelevance(SearchWords, input);
            return relevance / (float) SearchString.Length >= RELEVANCE_RATIO_MINIMUM;
        }
        
        private int ComputeRelevance(string[] searchWords, string input) 
        {
            string[] dishNameWords = input.ToLower().Split(" ");
            int relevance = 0;

            foreach (string searchWord in searchWords)
                relevance += computeRelevanceForWord(searchWord, dishNameWords);

            return relevance;
        }

        private int computeRelevanceForWord(string wordToSearch, IEnumerable<string> dishNameWords)
        {
            int finalScore = 0;

            foreach (string dishNameWord in dishNameWords) {
                int currentScore = maxOccurrence(wordToSearch, dishNameWord);

                if (currentScore > finalScore)
                    finalScore = currentScore;
            }


            return finalScore;
        }

        private int maxOccurrence(string contained, string container) 
        {
            int longestOccurrence = 0;

            for (int i = 0; i < container.Length; i++) {

                int currentOccurrence = 0;

                for (int i2 = 0; i2 < contained.Length && i2 + i < container.Length; i2++) {

                    if (contained[i2] == container[i + i2]) {
                        currentOccurrence++;
                    }
                }

                if (currentOccurrence > longestOccurrence) {
                    longestOccurrence = currentOccurrence;
                }
            }

            return longestOccurrence;
        }
        #endregion
    }
}
