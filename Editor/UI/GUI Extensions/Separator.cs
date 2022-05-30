using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public static partial class EditorGUILayoutExtension
    {
        /// <summary>
        /// Draws a horizontal separator, with margins. 
        /// </summary>
        /// <param name="thickness">The thickness of the separator</param>
        /// <param name="margin">The margin at the top and bottom of the separator</param>
        /// <returns>The position of the drawn separator</returns>
        public static Rect DrawHorizontalSeparator(float thickness, int margin = 0)
            => DrawHorizontalSeparator(thickness, margin, margin);

        /// <summary>
        /// Draws a horizontal separator, with margins. 
        /// </summary>
        /// <param name="thickness">The thickness of the separator</param>
        /// <param name="marginTop">The margin before the separator</param>
        /// <param name="marginBottom">The margin after the separator</param>
        /// <returns>The position of the drawn separator</returns>
        public static Rect DrawHorizontalSeparator(float thickness, int marginTop, int marginBottom)
        {
            float height = thickness;
            Rect controlRect = EditorGUILayout.GetControlRect(false, height, new GUIStyle {margin = new RectOffset(0, 0, marginTop, marginBottom)});
            EditorGUI.DrawRect(controlRect, SeparatorConstants.Color);

            return controlRect;
        }
    }

    public static partial class EditorGUIExtension
    {
        /// <summary>
        /// Draws a vertical separator, goes downward. 
        /// </summary>
        /// <param name="startingPosition">The point at the top of the separator</param>
        /// <param name="length">The length of the separator</param>
        /// <param name="thickness">The thickness of the separator</param>
        /// <returns>The position of the drawn separator</returns>
        public static Rect DrawVerticalSeparator(Vector2 startingPosition, float length, float thickness)
        {
            var halfThickness = ComputeHalfThickness(thickness);

            Rect separatorRect = new Rect(startingPosition.x - halfThickness, startingPosition.y, thickness, length);
            EditorGUI.DrawRect(separatorRect, SeparatorConstants.Color);

            return separatorRect;
        }


        /// <summary>
        /// Draws a horizontal separator, from left to right. 
        /// </summary>
        /// <param name="startingPosition">The point at the top of the separator</param>
        /// <param name="length">The length of the separator</param>
        /// <param name="thickness">The thickness of the separator</param>
        /// <returns>The position of the drawn separator</returns>
        public static Rect DrawHorizontalSeparator(Vector2 startingPosition, float length, float thickness)
        {
            var halfThickness = ComputeHalfThickness(thickness);
        
            Rect separatorRect = new Rect(startingPosition.x, startingPosition.y - halfThickness, length, thickness);
            EditorGUI.DrawRect(separatorRect, SeparatorConstants.Color);

            return separatorRect;
        }
    
        // This methods rounds the half value if the given value is an int
        private static float ComputeHalfThickness(float thickness)
        {
            var halfThickness = thickness / 2;
            int intThickness = Mathf.RoundToInt(thickness);
            if (Mathf.Abs(thickness - intThickness) < 0.05f)
            {
                // ReSharper disable once PossibleLossOfFraction
                halfThickness = intThickness / 2;
            }

            return halfThickness;
        }
    }
}

internal static class SeparatorConstants
{
    public static Color Color
    {
        get
        {
            if (EditorGUIUtility.isProSkin)
                return new Color(0.1f, 0.1f, 0.1f);
            
            return new Color(0.54f, 0.54f, 0.54f);
        }
    }
}
