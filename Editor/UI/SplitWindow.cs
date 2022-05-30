using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace HomaGames.GameDoctor.Ui
{
    public class SplitViewWindow : EditorWindow
    {
        private const int UpperNodeMargin = 7;
        private const int LowerNodeMargin = 7;
        private static readonly float TotalNodeSize = UpperNodeMargin + EditorGUIUtility.singleLineHeight + LowerNodeMargin;

        private IValidationProfile Profile = new ValidationProfile();

        private bool AutoFixToggle;
        private float HeaderSize;
        
        private SeparatedViewData SeparatedViewData;
        private Vector2 FirstViewScroll;
        private Vector2 SecondViewScroll;

        private bool ShowHiddenIssues;
        private string SearchString;

        #region UI Data
        private abstract class BaseUiData
        {
            protected readonly SplitViewWindow Window;
        
            private bool _selected;
            public bool Selected
            {
                get => _selected;
                set
                {
                    if (value)
                    {
                        foreach (var otherData in Window.GetAllUiData().Where(data => data != this))
                        {
                            otherData.Selected = false;
                        }

                        GUI.changed = true;
                    }

                    _selected = value;
                }
            }

            protected BaseUiData(SplitViewWindow window)
            {
                Window = window;
            }
        }
    
        private abstract class BaseFoldoutUiData : BaseUiData
        {
            public readonly AnimBool Expanded = new AnimBool(true);
        
            protected BaseFoldoutUiData(SplitViewWindow window) : base(window)
            {
                Expanded.valueChanged.AddListener(window.Repaint);
            }
        }
    
        private class ProfileUiData : BaseFoldoutUiData
        {
            public ProfileUiData(SplitViewWindow window) : base(window)
            {
            }
        }
    
        private class CheckUiData : BaseFoldoutUiData
        {
            public CheckUiData(SplitViewWindow window) : base(window)
            {
            }
        }

        private class IssueUiData : BaseUiData
        {
            public bool Hidden;
        
            public IssueUiData(SplitViewWindow window) : base(window)
            {
            }
        }

        private readonly Dictionary<IValidationProfile, ProfileUiData> ProfileUiDataBank = new Dictionary<IValidationProfile, ProfileUiData>();
        private readonly Dictionary<ICheck, CheckUiData> CheckUiDataBank = new Dictionary<ICheck, CheckUiData>();
        private readonly Dictionary<IIssue, IssueUiData> IssueUiDataBank = new Dictionary<IIssue, IssueUiData>();
        private float ScrollbarSize;
        private static readonly Color HighPriorityColor = new Color(0f, 0f, 0.49f);
        private static readonly Color MediumPriorityColor = Color.red;
        private static readonly Color LowPriorityColor = Color.yellow;

        private TValue TryGetOrCreate<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> creator)
        {
            if (dictionary.TryGetValue(key, out var output))
                return output;

            output = creator.Invoke();
            dictionary[key] = output;
            return output;
        }

        private ProfileUiData GetUiData(IValidationProfile profile)
            => TryGetOrCreate(ProfileUiDataBank, profile, () => new ProfileUiData(this));

        private CheckUiData GetUiData(ICheck check)
            => TryGetOrCreate(CheckUiDataBank, check, () => new CheckUiData(this));

        private IssueUiData GetUiData(IIssue issue)
            => TryGetOrCreate(IssueUiDataBank, issue, () => new IssueUiData(this));

        private IEnumerable<BaseUiData> GetAllUiData()
        {
            return ProfileUiDataBank.Values.Union<BaseUiData>(CheckUiDataBank.Values).Union(IssueUiDataBank.Values);
        }
        #endregion

        [MenuItem("Test/Split View")]
        public static void Init()
        {
            GetWindow<SplitViewWindow>().Show();
        }

        private void OnEnable()
        {
            Profile.CheckList.Add(new SimpleCheck(
                async () =>
                {
                    await Task.Delay(1000);
                    return new CheckResult
                    {
                        Issues = new List<IIssue>
                        {
                            new SimpleIssue(async () => { await Task.Delay(200); }, "issue 1", ""),
                            new SimpleIssue(async () => { await Task.Delay(200); }, "issue 2", ""),
                            new SimpleIssue(async () => { await Task.Delay(200); }, "issue 3", ""),
                        }
                    };
                }, "Check 1", "description", new List<string>()
            ));
        }

        void OnGUI()
        {
            float footerSize = 150;
            DrawHeader();

            SeparatedViewData = EditorGUILayoutExtension.BeginSeparatedView(position.height - HeaderSize - footerSize,
                SeparatedViewData, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(position.height - HeaderSize - footerSize));

            EditorGUILayoutExtension.BeginToolBar();
            SearchString = EditorGUILayoutExtension.ToolBarSearchBar(SearchString, GUILayout.ExpandWidth(true));
            ShowHiddenIssues = EditorGUILayoutExtension.ToolBarToggle(ShowHiddenIssues, "Show hidden");
            EditorGUILayoutExtension.EndToolBar();
        
            EditorGUILayoutExtension.BeginToolBar();
            EditorGUILayoutExtension.ToolBarButton("X");
            EditorGUILayoutExtension.ToolBarButton("Name", GUILayout.ExpandWidth(true));
            EditorGUILayoutExtension.ToolBarButton("X", GUILayout.Width(TotalNodeSize));
            EditorGUILayoutExtension.ToolBarButton("X", GUILayout.Width(TotalNodeSize));
            EditorGUILayoutExtension.ToolBarButton("X", GUILayout.Width(TotalNodeSize));
            EditorGUILayoutExtension.ToolBarSpace(ScrollbarSize);
            EditorGUILayoutExtension.EndToolBar();

            float scrollViewWidth = GetCurrentLayoutWidth();
            FirstViewScroll = EditorGUILayout.BeginScrollView(FirstViewScroll);
            UpdateScrollViewWidth(scrollViewWidth);
            DrawNode(Profile);
            EditorGUILayout.EndScrollView();
        
            SeparatedViewData = EditorGUILayoutExtension.PutSeparatorInView(SeparatedViewData, 200, 200);

            SecondViewScroll = EditorGUILayout.BeginScrollView(SecondViewScroll);
            GUILayout.Label("Description");
            EditorGUILayout.EndScrollView();
        

            EditorGUILayoutExtension.EndSeparatedView();
            EditorGUILayoutExtension.DrawHorizontalSeparator(1);
            DrawFooter(footerSize);
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

        // This method returns a valid value only on EventType.Repaint events
        private float GetCurrentLayoutWidth() => EditorGUILayout.GetControlRect(false, 0, GUIStyle.none).width;

        private void DrawHeader()
        {
            int textSize = 28;
            int padding = Mathf.RoundToInt(EditorGUIUtility.singleLineHeight / 2);
            GUILayout.BeginHorizontal(new GUIStyle
            {
                margin = new RectOffset(padding, padding, padding, padding)
            });

            GUIContent titleGuiContent = new GUIContent("Game Doctor");
            var titleGuiStyle = new GUIStyle(EditorStyles.largeLabel) { fontStyle = FontStyle.Bold, fontSize = textSize };
            GUILayout.Label(titleGuiContent, titleGuiStyle);
            Rect labelRect = GUILayoutUtility.GetRect(titleGuiContent, titleGuiStyle); 
        
            GUILayout.FlexibleSpace();

            GUIStyle controlStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16,
                margin = {top = 10}
            };

            AutoFixToggle = GUILayout.Toggle(AutoFixToggle, "Auto-Fix", new GUIStyle(controlStyle) {margin = {right = 0}});
            if (GUILayout.Button("Scan", new GUIStyle(controlStyle) {margin = {left = 0}}))
            {
                Scan();
            }
            GUILayout.Space(15);
        
            GUI.enabled = false;
            GUILayout.Button("Fix Auto. issues", new GUIStyle(controlStyle));
            GUI.enabled = true;
        
            GUILayout.EndHorizontal();
            EditorGUILayoutExtension.DrawHorizontalSeparator(1);

            if (Event.current.type == EventType.Repaint) 
                HeaderSize = Mathf.CeilToInt(labelRect.height) + 2 * padding + titleGuiStyle.margin.top + titleGuiStyle.margin.bottom;
        }

        private async Task Scan()
        {
            List<ICheck> checks = Profile.CheckList;

            float maxI = checks.Count;
            string scanWindowTitle = "Executing all checks";
            string scanWindowContent = "Looking for issues in the project";

            try
            {
                EditorUtility.DisplayProgressBar(scanWindowTitle, scanWindowContent, 0);
                for (int i = 0; i < maxI; i++)
                {
                    try
                    {
                        await checks[i].Execute();
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Exception when running \"{checks[i].Name}\" check:\n{e}");
                    }
                    EditorUtility.DisplayProgressBar(scanWindowTitle, scanWindowContent, i / maxI);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
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
            
            DrawFoldoutTreeElement(uiData, check.Name, check.GetPriorityCount(), () =>
            {

                if (check.CheckResult != null)
                {
                    foreach (var issue in check.CheckResult.Issues)
                    {
                        DrawNode(issue);
                    }
                }
            }, check.CheckResult != null && !check.CheckResult.Passed);
        }

        private void DrawFoldoutTreeElement(BaseFoldoutUiData uiData, string nodeName, PriorityCount priorityCount, Action drawInside, bool drawAsFoldout = true)
        {
            DrawNodeBefore(uiData);
            if (drawAsFoldout)
                uiData.Expanded.target = EditorGUILayout.Foldout(uiData.Expanded.target, nodeName);
            else
                EditorGUILayout.LabelField(nodeName);

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            Rect priorityRect = EditorGUILayout.GetControlRect(GUILayout.Width(TotalNodeSize * 3 - EditorGUIUtility.standardVerticalSpacing));
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
                DrawNodeAfter(uiData);
            }
        }

        private static GUIStyle SelectedNodeStyle => new GUIStyle("OL SelectedRow")
            {padding = new RectOffset(0, 0, UpperNodeMargin, LowerNodeMargin), margin = new RectOffset()};
        private static GUIStyle RegularNodeStyle => new GUIStyle()
            {padding = new RectOffset(0, 0, UpperNodeMargin, LowerNodeMargin), margin = new RectOffset()};
    
        private static void DrawNodeBefore(BaseUiData uiData)
        {
            EditorGUILayout.BeginHorizontal(uiData.Selected ? SelectedNodeStyle : RegularNodeStyle, GUILayout.ExpandWidth(true));
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
            }
        }
    
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