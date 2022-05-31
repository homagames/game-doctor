using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public static partial class EditorGUILayoutExtension
    {
        /// <summary>
        /// Starts the drawing of a toolbar.
        /// </summary>
        /// <seealso cref="ToolBarButton(GUIContent, GUILayoutOption[])"/>
        /// <seealso cref="ToolBarToggle(bool, GUIContent, GUILayoutOption[])"/>
        /// <seealso cref="ToolBarSearchBar(string, GUILayoutOption[])"/>
        /// <seealso cref="EndToolBar"/>
        public static void BeginToolBar()
        {
            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
        }

        /// <summary>
        /// Draws a button in a toolbar.
        /// </summary>
        /// <param name="label">The label for the button</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.<br /> See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>True when the button is being clicked on</returns>
        /// <seealso cref="BeginToolBar"/>
        /// <seealso cref="EndToolBar"/>
        public static bool ToolBarButton(string label, params GUILayoutOption[] options) => ToolBarButton(new GUIContent(label), options);
    
        /// <summary>
        /// Draws a button in a toolbar.
        /// </summary>
        /// <param name="content">The content for the button</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.<br /> See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>True when the button is being clicked on</returns>
        /// <seealso cref="BeginToolBar"/>
        /// <seealso cref="EndToolBar"/>
        public static bool ToolBarButton(GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.Button(content, EditorStyles.toolbarButton, options.Length == 0 ? new []{GUILayout.ExpandWidth(false)} : options);
        }

        /// <summary>
        /// Draws a button that will toggle on each press in a toolbar.
        /// </summary>
        /// <param name="value">The state of the button. The return value of this function should be assigned back to this parameter.</param>
        /// <param name="label">The label for the button</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.<br /> See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>True when the toggle is active</returns>
        /// <seealso cref="BeginToolBar"/>
        /// <seealso cref="EndToolBar"/>
        public static bool ToolBarToggle(bool value, string label, params GUILayoutOption[] options) => ToolBarToggle(value, new GUIContent(label), options);
        /// <summary>
        /// Draws a button that will toggle on each press in a toolbar.
        /// </summary>
        /// <param name="value">The state of the button. The return value of this function should be assigned back to this parameter.</param>
        /// <param name="content">The content for the button</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.<br /> See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>True when the toggle is active</returns>
        /// <seealso cref="BeginToolBar"/>
        /// <seealso cref="EndToolBar"/>
        public static bool ToolBarToggle(bool value, GUIContent content, params GUILayoutOption[] options)
        {
            return GUILayout.Toggle(value, content, EditorStyles.toolbarButton, options.Length == 0 ? new []{GUILayout.ExpandWidth(false)} : options);
        }

        public static void ToolBarSpace(float size)
        {
            EditorGUILayout.GetControlRect(false, 2, GUIStyle.none, GUILayout.Width(size));
        }
    
        public static void ToolBarLabel(string label, params GUILayoutOption[] options)
            => ToolBarLabel(new GUIContent(label), options);

        public static void ToolBarLabel(GUIContent content, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(content, EditorStyles.toolbar, options.Length == 0 ? new []{GUILayout.ExpandWidth(false)} : options);
        }
    
        /// <summary>
        /// Draws a text field as a search bar, in a tool bar.
        /// </summary>
        /// <param name="searchString">Text to edit. The return value of this function should be assigned back to the string.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.<br /> See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>The edited string.</returns>
        /// <seealso cref="BeginToolBar"/>
        /// <seealso cref="EndToolBar"/>
        public static string ToolBarSearchBar(string searchString, params GUILayoutOption[] options)
            => SearchBar(searchString, true, options);

        /// <summary>
        /// Ends the drawing of a toolbar.
        /// </summary>
        /// <seealso cref="BeginToolBar"/>
        /// <seealso cref="ToolBarButton(GUIContent, GUILayoutOption[])"/>
        /// <seealso cref="ToolBarToggle(bool, GUIContent, GUILayoutOption[])"/>
        /// <seealso cref="ToolBarSearchBar(string, GUILayoutOption[])"/>
        public static void EndToolBar()
        {
            GUILayout.EndHorizontal();
        }
    }
}