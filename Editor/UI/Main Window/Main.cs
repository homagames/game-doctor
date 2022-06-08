using System.Collections.Generic;
using System.Linq;
using HomaGames.GameDoctor.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class GameDoctorWindow : EditorWindow
    {
        // Non-Breakable SPace, to prevent trimming 
        private const string NBSP = " ";
        
        private const int UpperNodeMargin = 7;
        private const int LowerNodeMargin = 7;
        private static readonly float TotalNodeSize = UpperNodeMargin + EditorGUIUtility.singleLineHeight + LowerNodeMargin;
        
        private static Color HighPriorityColor => Color.red;
        private static Color MediumPriorityColor => new Color(0.87f, 0.49f, 0.16f);
        private static Color LowPriorityColor => EditorGUIUtility.isProSkin ? Color.yellow : new Color(0.87f, 1f, 0.31f);
        
        public static bool IsProfileOpened { get; private set; }

        private string _validationProfileName;
        private IValidationProfile _profile;

        private IValidationProfile Profile
        {
            get
            {
                if (_profile == null)
                {
                    AvailableProfiles.GetDefaultValidationProfile();
                    
                    _profile = AvailableProfiles
                        .GetAllValidationProfiles()
                        .FirstOrDefault(p => p.Name == _validationProfileName);

                    if (_profile == null)
                    {
                        Debug.LogError($"To open the Game Doctor window, use {nameof(GameDoctorWindow)}.{nameof(Open)}({nameof(IValidationProfile)})");
                        Close();
                    }
                }

                return _profile;
            }
            set
            {
                _profile = value;
                _validationProfileName = _profile.Name;
            }
        }

        private SeparatedViewData SeparatedViewData;
        
        private Texture2D MandatoryTexture;
        
        private Texture2D HighPriorityTexture;
        private Texture2D MediumPriorityTexture;
        private Texture2D LowPriorityTexture;
        
        private Texture2D HighPriorityWhiteTexture;
        private Texture2D MediumPriorityWhiteTexture;
        private Texture2D LowPriorityWhiteTexture;
        
        private Texture2D ProfileTexture;
        private Texture2D CheckTexture;
        
        private Texture2D AutomaticTexture;
        private Texture2D InteractiveTexture;
        
        private Texture2D FixedColoredTexture;
        private Texture2D FixedWhiteTexture;

        private void OnEnable()
        {
            IsProfileOpened = true;
            titleContent = new GUIContent("Game Doctor");
        }

        private void OnGUI()
        {
            if (Profile == null)
            {
                Debug.LogError($"To open the Game Doctor window, use {nameof(GameDoctorWindow)}.{nameof(Open)}({nameof(IValidationProfile)})");
                Close();
            }
            
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

        private void OnDestroy()
        {
            IsProfileOpened = false;
        }

        // This method returns a valid value only on EventType.Repaint events
        [MustUseReturnValue]
        private float GetCurrentLayoutWidth() => EditorGUILayout.GetControlRect(false, 0, GUIStyle.none).width;
        
        [Pure]
        [NotNull]
        private IEnumerable<IIssue> GetAllIssues()
        {
            return Profile.CheckList.SelectMany(check => check.CheckResult?.Issues ?? Enumerable.Empty<IIssue>());
        }

        [Pure]
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
         
        [NotNull]
        [Pure]
        private GUIContent GetColorableGuiContentFor(Priority priority)
        {
            switch (priority)
            {
                default:
                case Priority.Low:
                    return new GUIContent(NBSP + "Low", LowPriorityWhiteTexture);
                case Priority.Medium:
                    return new GUIContent(NBSP + "Medium", MediumPriorityWhiteTexture);
                case Priority.High:
                    return new GUIContent(NBSP + "High", HighPriorityWhiteTexture);
            }
        }
        
        [NotNull]
        [Pure]
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
        
        [Pure]
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

        [NotNull]
        [Pure]
        private GUIContent GetGuiContentFor(AutomationType automationType)
        {
            switch (automationType)
            {
                default:
                case AutomationType.Interactive:
                    return new GUIContent(NBSP + "Interactive", InteractiveTexture);
                case AutomationType.Automatic:
                    return new GUIContent(NBSP + "Automatic", AutomaticTexture);
            }
        }

        [NotNull]
        [Pure]
        private Texture2D GetTextureFor(AutomationType automationType)
        {
            switch (automationType)
            {
                default:
                case AutomationType.Interactive:
                    return InteractiveTexture;
                case AutomationType.Automatic:
                    return AutomaticTexture;
            }
        }
    }
}