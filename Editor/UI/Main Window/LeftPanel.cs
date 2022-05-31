using System;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class SplitViewWindow
    {
        private Vector2 FirstViewScroll;
        private float ScrollbarSize;
        
        private bool ShowHiddenIssues;
        private string SearchString;

        private Texture2D MandatoryTexture;
        
        private Texture2D HighPriorityTexture;
        private Texture2D MediumPriorityTexture;
        private Texture2D LowPriorityTexture;

        private void DrawLeftPanel()
        {
            EditorGUILayoutExtension.BeginToolBar();
            SearchString = EditorGUILayoutExtension.ToolBarSearchBar(SearchString, GUILayout.ExpandWidth(true));
            ShowHiddenIssues = EditorGUILayoutExtension.ToolBarToggle(ShowHiddenIssues, "Show hidden");
            EditorGUILayoutExtension.EndToolBar();

            EditorGUILayoutExtension.BeginToolBar();
            EditorGUILayoutExtension.ToolBarButton(new GUIContent(MandatoryTexture), GUILayout.MaxWidth(30));
            EditorGUILayoutExtension.ToolBarButton("Name", GUILayout.ExpandWidth(true));
            EditorGUILayoutExtension.ToolBarButton(new GUIContent(HighPriorityTexture), GUILayout.Width(TotalNodeSize));
            EditorGUILayoutExtension.ToolBarButton(new GUIContent(MediumPriorityTexture), GUILayout.Width(TotalNodeSize));
            EditorGUILayoutExtension.ToolBarButton(new GUIContent(LowPriorityTexture), GUILayout.Width(TotalNodeSize));
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
            DrawFoldoutTreeElement(uiData, profile.Name, profile.GetPriorityCount(), () =>
            {
                foreach (var check in profile.CheckList)
                {
                    DrawNode(check);
                }
            });
        }

        private void DrawNode(ICheck check)
        {
            CheckUiData uiData = GetUiData(check);

            var containsIssues = check.CheckResult != null && !check.CheckResult.Passed;
            DrawFoldoutTreeElement(uiData, check.Name, check.GetPriorityCount(), () =>
            {

                if (check.CheckResult != null)
                {
                    foreach (var issue in check.CheckResult.Issues)
                    {
                        DrawNode(issue);
                    }
                }
            }, containsIssues, containsIssues && check.Importance == ImportanceType.Mandatory);
        }

        private void DrawFoldoutTreeElement(BaseFoldoutUiData uiData, string nodeName, PriorityCount priorityCount,
            Action drawInside, bool drawAsFoldout = true, bool drawMandatory = false)
        {
            DrawNodeBefore(uiData);
            if (drawAsFoldout)
                uiData.Expanded.target = EditorGUILayout.Foldout(uiData.Expanded.target, nodeName);
            else
                EditorGUILayout.LabelField(nodeName);
            
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
            if (ShowHiddenIssues || !uiData.Hidden)
            {
                DrawNodeBefore(uiData);
                EditorGUILayout.LabelField(issue.Name);

                var priorityIconGUIStyle = new GUIStyle(GUIStyle.none) { margin = new RectOffset(0, 7, 0, 0)};
                GUI.DrawTexture(
                    EditorGUILayout.GetControlRect(
                        false, EditorGUIUtility.singleLineHeight, 
                        priorityIconGUIStyle, GUILayout.Width(16)), 
                    HighPriorityTexture);
                
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
    }
}
