using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public static partial class EditorGUILayoutExtension
    {
        private static readonly GUIStyle SearchBarToolbarGUIStyle = new GUIStyle(GUI.skin.FindStyle("Toolbar"));
        private static readonly GUIStyle SearchBarTextGUIStyle = new GUIStyle(GUI.skin.FindStyle("ToolbarSeachTextField"));
        private static readonly GUIStyle SearchBarCancelGUIStyle = new GUIStyle(GUI.skin.FindStyle("ToolbarSeachCancelButton"));

        /// <summary>
        /// Draws a text field as a search bar, in a tool bar.
        /// </summary>
        /// <param name="searchString">Text to edit. The return value of this function should be assigned back to the string.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.<br /> See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>The edited string. </returns>
        public static string SearchBar(string searchString, params GUILayoutOption[] options)
            => SearchBar(searchString, false, options);

        private static string SearchBar(string searchString, bool inToolBar, params GUILayoutOption[] options)
        {
            if (SearchBarCancelGUIStyle == null
                || SearchBarTextGUIStyle == null
                || SearchBarToolbarGUIStyle == null)
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

            return searchString;
        }
    }
}