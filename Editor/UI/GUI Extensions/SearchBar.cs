using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public static partial class EditorGUILayoutExtension
    {
        [NotNull]
        private static GUIStyle SearchBarToolbarGUIStyle => EditorStyles.toolbar;
        [NotNull]
        private static GUIStyle SearchBarTextGUIStyle => EditorStyles.toolbarSearchField;
        [CanBeNull]
        private static GUIStyle SearchBarCancelGUIStyle => new GUIStyle(GUI.skin.FindStyle("ToolbarSeachCancelButton"));

        /// <summary>
        /// Draws a text field as a search bar, in a tool bar.
        /// </summary>
        /// <param name="searchString">Text to edit. The return value of this function should be assigned back to the string.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.<br /> See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>The edited string. </returns>
        [NotNull]
        public static string SearchBar([NotNull] string searchString, [NotNull] params GUILayoutOption[] options)
            => SearchBar(searchString, false, options);

        [NotNull]
        private static string SearchBar([NotNull] string searchString, bool inToolBar, [NotNull] params GUILayoutOption[] options)
        {
            if (SearchBarCancelGUIStyle == null)
            {
                return GUILayout.TextField(searchString, options);
            }
        
            if (! inToolBar)
                GUILayout.BeginHorizontal(SearchBarToolbarGUIStyle, options);

            var textFieldOptions = inToolBar ? options : new [] { GUILayout.ExpandWidth(true) };
            searchString = EditorGUILayout.TextField(searchString, SearchBarTextGUIStyle, textFieldOptions);
        
            if (! string.IsNullOrEmpty(searchString))
            {
                if (GUILayout.Button("", SearchBarCancelGUIStyle))
                {
                    // Remove focus if cleared
                    searchString = "";
                    GUI.FocusControl(null);
                }
            }
        
            if (! inToolBar)
                GUILayout.EndHorizontal();
            
            return searchString ?? "";
        }
    }
}