using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class SplitViewWindow : EditorWindow
    {
        private const int UpperNodeMargin = 7;
        private const int LowerNodeMargin = 7;
        private static readonly float TotalNodeSize = UpperNodeMargin + EditorGUIUtility.singleLineHeight + LowerNodeMargin;
        
        private static readonly Color HighPriorityColor = new Color(0f, 0f, 0.49f);
        private static readonly Color MediumPriorityColor = Color.red;
        private static readonly Color LowPriorityColor = Color.yellow;

        private IValidationProfile Profile = new ValidationProfile();

        private SeparatedViewData SeparatedViewData;
        private Vector2 SecondViewScroll;

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

        // TODO: have a proper initialization method
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
                            new SimpleIssue(async () => { await Task.Delay(200); }, "issue 1", "description 1"),
                            new SimpleIssue(async () => { await Task.Delay(200); }, "issue 2", ""),
                            new SimpleIssue(async () => { await Task.Delay(200); }, "issue 3", ""),
                        }
                    };
                }, "Check 1", "description", new List<string>(), ImportanceType.Mandatory
            ));

            MandatoryTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Doctor/hg-mobile-unitypackage-game-doctor/Editor/UI/Main Window/Icons/MandatoryIcon.png");
            
            HighPriorityTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Doctor/hg-mobile-unitypackage-game-doctor/Editor/UI/Main Window/Icons/HighPriorityIconPro.png");
            MediumPriorityTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Doctor/hg-mobile-unitypackage-game-doctor/Editor/UI/Main Window/Icons/MediumPriorityIconPro.png");
            LowPriorityTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Doctor/hg-mobile-unitypackage-game-doctor/Editor/UI/Main Window/Icons/LowPriorityIconPro.png");
        }

        void OnGUI()
        {
            float footerSize = 150;
            DrawHeader();

            SeparatedViewData = EditorGUILayoutExtension.BeginSeparatedView(position.height - HeaderSize - footerSize,
                SeparatedViewData, GUILayout.ExpandWidth(true), GUILayout.MaxHeight(position.height - HeaderSize - footerSize));

            DrawLeftPanel();

            SeparatedViewData = EditorGUILayoutExtension.PutSeparatorInView(SeparatedViewData, 200, 200);

            DrawRightPanel();


            EditorGUILayoutExtension.EndSeparatedView();
            EditorGUILayoutExtension.DrawHorizontalSeparator(1);
            DrawFooter(footerSize);
        }

        // This method returns a valid value only on EventType.Repaint events
        private float GetCurrentLayoutWidth() => EditorGUILayout.GetControlRect(false, 0, GUIStyle.none).width;
    }
}