using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using JetBrains.Annotations;
using UnityEditor;
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
        
        private Texture2D MandatoryTexture;
        
        private Texture2D HighPriorityTexture;
        private Texture2D MediumPriorityTexture;
        private Texture2D LowPriorityTexture;
        
        private Texture2D AutomaticTexture;
        private Texture2D InteractiveTexture;
        
        private Texture2D FixedTexture;

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
                            new SimpleIssue(async () => { await Task.Delay(200); }, "issue 1", "description 1", AutomationType.Automatic, Priority.High),
                            new SimpleIssue(async () => { await Task.Delay(200); }, "issue 2", "", AutomationType.Automatic, Priority.Medium),
                            new SimpleIssue(async () => { await Task.Delay(200); }, "issue 3", "", AutomationType.Automatic, Priority.Low),
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
            InteractiveTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Doctor/hg-mobile-unitypackage-game-doctor/Editor/UI/Main Window/Icons/InteractiveIconPro.png");
            AutomaticTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Doctor/hg-mobile-unitypackage-game-doctor/Editor/UI/Main Window/Icons/AutomaticIconPro.png");
            
            FixedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Assets/Doctor/hg-mobile-unitypackage-game-doctor/Editor/UI/Main Window/Icons/FixedIcon.png");
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
        
        private IEnumerable<IIssue> GetAllIssues()
        {
            return Profile.CheckList.SelectMany(check => check.CheckResult?.Issues ?? Enumerable.Empty<IIssue>());
        }

        [CanBeNull]
        private object GetSelectedElement()
        {
            foreach (var profileUiDataPair in ProfileUiDataBank)
            {
                if (profileUiDataPair.Value.Selected)
                    return profileUiDataPair.Key;
            }
            
            foreach (var checkUiDataPair in CheckUiDataBank)
            {
                if (checkUiDataPair.Value.Selected)
                    return checkUiDataPair.Key;
            }
            
            foreach (var issueUiDataPair in IssueUiDataBank)
            {
                if (issueUiDataPair.Value.Selected)
                    return issueUiDataPair.Key;
            }

            return null;
        }
        
        
        private GUIContent GetGuiContentFor(Priority priority)
        {
            switch (priority)
            {
                default:
                case Priority.Low:
                    return new GUIContent("Low", LowPriorityTexture);
                case Priority.Medium:
                    return new GUIContent("Medium", MediumPriorityTexture);
                case Priority.High:
                    return new GUIContent("High", HighPriorityTexture);
            }
        }
        
        private Texture2D GetTextureFor(Priority priority)
        {
            switch (priority)
            {
                default:
                case Priority.Low:
                    return LowPriorityTexture;
                case Priority.Medium:
                    return MediumPriorityTexture;
                case Priority.High:
                    return HighPriorityTexture;
            }
        }
        
        private Color GetColorFor(Priority priority)
        {
            switch (priority)
            {
                default:
                case Priority.Low:
                    return LowPriorityColor;
                case Priority.Medium:
                    return MediumPriorityColor;
                case Priority.High:
                    return HighPriorityColor;
            }
        }

        private GUIContent GetGuiContentFor(AutomationType automationType)
        {
            switch (automationType)
            {
                default:
                case AutomationType.Interactive:
                    return new GUIContent("Interactive", InteractiveTexture);
                case AutomationType.Automatic:
                    return new GUIContent("Automatic", AutomaticTexture);
            }
        }
    }
}