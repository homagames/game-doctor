using System;
using System.Collections.Generic;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public class ProfileSelectorPopup : EditorWindow
    {
        public static void Open()
        {
            GetWindow<ProfileSelectorPopup>().ShowModalUtility();
        }

        private List<IValidationProfile> ProfileList;
        private const float HeaderSpacingBefore = 10;
        private const float HeaderSpacingAfter = 10;
        private const float WindowSpacingAfter = 10;

        private void OnEnable()
        {
            ProfileList = AvailableProfiles.GetAllValidationProfiles();
            
            position = new Rect(position)
            {
                size = new Vector2(300,
                    HeaderSpacingBefore + HeaderSpacingAfter + WindowSpacingAfter +
                    (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) *
                    (ProfileList.Count + 1))
            };
        }

        void OnGUI()
        {
            EditorGUILayout.Space(HeaderSpacingBefore);
            EditorGUILayout.LabelField("Select a profile:", EditorStyles.boldLabel);
            EditorGUILayout.Space(HeaderSpacingAfter);
        
            foreach (var profile in ProfileList)
            {
                if (GUILayout.Button(profile.Name))
                {
                    GameDoctorWindow.Open(profile);
                    Close();
                }
            }
        
            EditorGUILayout.Space(WindowSpacingAfter);
        }
    }
}