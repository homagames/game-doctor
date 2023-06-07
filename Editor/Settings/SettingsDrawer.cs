using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public static class SettingsDrawer
    {
        private static ReorderableList _uiList;
        private static readonly Dictionary<TagBasedValidationProfile, ReorderableList> TagUiLists 
            = new Dictionary<TagBasedValidationProfile, ReorderableList>();

        private static ReorderableList GetTagUiList(TagBasedValidationProfile profile)
        {
            if (TagUiLists.TryGetValue(profile, out var list))
                return list;

            list = new ReorderableList(profile.Tags, typeof(string))
            {
                drawElementCallback = CreateDrawTagElementCallback(profile),
                drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Tags")
            };
            TagUiLists.Add(profile, list);
            return list;
        }

        private static ReorderableList.ElementCallbackDelegate CreateDrawTagElementCallback(TagBasedValidationProfile profile)
        {
            return (rect, index, isActive, isFocused) =>
            {
                var tagList = AvailableChecks.GetAllTags();
                var currentTag = profile.Tags[index];
                int currentIndex = string.IsNullOrEmpty(currentTag) ? 0 : tagList.IndexOf(currentTag);
                
                if (tagList.Count > 0 && currentIndex != -1)
                {
                    currentIndex = EditorGUI.Popup(rect, currentIndex, tagList.Append("Enter custom tag...").ToArray());
                    if (currentIndex >= tagList.Count)
                    {
                        currentTag = " ";
                        while (tagList.Contains(currentTag)) currentTag += ' ';
                    }
                    else
                        currentTag = tagList[currentIndex];
                }
                else
                {
                    currentTag = EditorGUI.DelayedTextField(rect, currentTag);
                }

                profile.Tags[index] = currentTag;
            };
        }

        public static void Draw()
        {
            EditorGUI.BeginChangeCheck();
            if (_uiList == null)
            {
                _uiList = new ReorderableList(SettingsAsset.instance.ProfileList,
                    typeof(TagBasedValidationProfile))
                {
                    elementHeightCallback = ElementHeightCallback,
                    drawElementCallback = DrawElementCallback,
                    drawElementBackgroundCallback = DrawElementBackground,
                    drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Preset Profiles")
                };
            }
            
            EditorGUILayout.Space(10);

            _uiList.DoLayoutList();
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.HelpBox("This feature only propose tag based profiles, but you can " +
                                    $"implement you own custom profiles by implementing {nameof(IValidationProfile)}, and " +
                                    $"subscribing instances to {nameof(AvailableProfiles)}", MessageType.Info);
            if (!EditorGUI.EndChangeCheck()) return;
            SettingsAsset.instance.Save();
        }

        private static float ElementHeightCallback(int index)
        {
            if (SettingsAsset.instance.ProfileList[index] != null)
                return GetTagUiList(SettingsAsset.instance.ProfileList[index]).GetHeight() + EditorGUIUtility.singleLineHeight * 6
                                                                + EditorGUIUtility.standardVerticalSpacing * 5;
            return 0;
        }

        private static void DrawElementCallback(Rect rect, int index, bool isActive, bool isFocused)
        {
            TagBasedValidationProfile profile = SettingsAsset.instance.ProfileList[index];

            void MoveDown(ref Rect rectangle, float y)
            {
                rectangle.y += y;
                rectangle.height -= y;
            }
            
            MoveDown(ref rect, EditorGUIUtility.standardVerticalSpacing);
            
            EditorGUI.LabelField(new Rect(rect) {height = EditorGUIUtility.singleLineHeight}, profile.Name, EditorStyles.boldLabel);
            MoveDown(ref rect, EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
            
            profile.Name = EditorGUI.TextField(new Rect(rect) {height = EditorGUIUtility.singleLineHeight}, "Name", profile.Name);
            MoveDown(ref rect, EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);

            EditorGUI.LabelField(new Rect(rect) {height = EditorGUIUtility.singleLineHeight}, "Description");
            MoveDown(ref rect, EditorGUIUtility.singleLineHeight);
            profile.Description = EditorGUI.TextArea(new Rect(rect) {height = EditorGUIUtility.singleLineHeight * 3}, profile.Description);
            MoveDown(ref rect, EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing);

            rect.height -= EditorGUIUtility.standardVerticalSpacing;
            GetTagUiList(profile).DoList(rect);
        }

        private static Color BackgroundColor => 
            EditorGUIUtility.isProSkin ? 
                new Color(0.22f, 0.22f, 0.22f) : 
                new Color(0.73f, 0.73f, 0.73f);

        private static Color AccentBackgroundColor => 
            EditorGUIUtility.isProSkin ? 
                new Color(0.25f, 0.25f, 0.25f) : 
                new Color(0.78f, 0.78f, 0.78f);

        private static void DrawElementBackground(Rect rect, int index, bool isActive, bool isFocused) {
            rect.width -= 2;
            if (isActive || isFocused) {
                Color color = new Color(0.27f, 0.38f, 0.48f);
                if (isActive) {
                    color.a = 0.2f;
                }
                if (isFocused) {
                    color.a = 0.9f;
                }

                EditorGUI.DrawRect(rect, color);
            }
            else
            {
                EditorGUI.DrawRect(rect, (index % 2 == 1) ? BackgroundColor : AccentBackgroundColor);
            }
        }
    }
}