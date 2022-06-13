using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public static partial class EditorGUILayoutExtension
    {
        /// <summary>
        /// Draws a pie chart with labels on the right. 
        /// </summary>
        /// <param name="height">The height of the pie chart</param>
        /// <param name="values">The values to display in the chart</param>
        public static void DrawPieChart(float height, [NotNull] params EditorGUIExtension.PieChartValue[] values)
            => DrawPieChart(height, true, values);

        /// <summary>
        /// Draws a pie chart with labels on the right. 
        /// </summary>
        /// <param name="height">The height of the pie chart</param>
        /// <param name="values">The values to display in the chart</param>
        public static void DrawPieChart(float height, [NotNull] IReadOnlyList<EditorGUIExtension.PieChartValue> values)
            => DrawPieChart(height, true, values);

        /// <summary>
        /// Draws a pie chart. 
        /// </summary>
        /// <param name="height">The height of the pie chart</param>
        /// <param name="showLabels">Whether or not to draw the labels</param>
        /// <param name="values">The values to display in the chart</param>
        public static void DrawPieChart(float height, bool showLabels, [NotNull] params EditorGUIExtension.PieChartValue[] values)
            => DrawPieChart(height, showLabels, (IReadOnlyList<EditorGUIExtension.PieChartValue>) values);

        /// <summary>
        /// Draws a pie chart. 
        /// </summary>
        /// <param name="height">The height of the pie chart</param>
        /// <param name="showLabels">Whether or not to draw the labels</param>
        /// <param name="values">The values to display in the chart</param>
        public static void DrawPieChart(float height, bool showLabels, [NotNull] IReadOnlyList<EditorGUIExtension.PieChartValue> values)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUIExtension.DrawPieChart(rect, showLabels, values);
        }
    }

    public static partial class EditorGUIExtension
    {
        /// <summary>
        /// A struct to represent a quadrant of the pie chart
        /// </summary>
        /// <seealso cref="EditorGUIExtension.DrawPieChart(Rect, bool, PieChartValue[])"/>
        /// <seealso cref="EditorGUILayoutExtension.DrawPieChart(float, bool, PieChartValue[])"/>
        public class PieChartValue
        {
            /// <summary>
            /// The weight of the element in the chart
            /// </summary>
            public float Value;
            /// <summary>
            /// The color to use to display the chart
            /// </summary>
            public Color Color;
            /// <summary>
            /// The label of the element. If all labels of a chart are null, the legend will be hidden.
            /// </summary>
            [CanBeNull]
            public string Label;
            /// <summary>
            /// The thickness of the arc, between 0 and 10.
            /// </summary>
            public float Thickness = 1;
        }
    
        /// <summary>
        /// Draws a pie chart. 
        /// </summary>
        /// <param name="rect">The position of the pie chart</param>
        /// <param name="values">The values to display in the chart</param>
        public static void DrawPieChart(Rect rect, params PieChartValue[] values)
            => DrawPieChart(rect, true, values);

        /// <summary>
        /// Draws a pie chart. 
        /// </summary>
        /// <param name="rect">The position of the pie chart</param>
        /// <param name="values">The values to display in the chart</param>
        public static void DrawPieChart(Rect rect, [NotNull] IReadOnlyList<PieChartValue> values)
            => DrawPieChart(rect, true, values);

        /// <summary>
        /// Draws a pie chart. 
        /// </summary>
        /// <param name="rect">The position of the pie chart</param>
        /// <param name="showLabels">Whether or not to draw the labels</param>
        /// <param name="values">The values to display in the chart</param>
        public static void DrawPieChart(Rect rect, bool showLabels, [NotNull] params PieChartValue[] values)
            => DrawPieChart(rect, showLabels, (IReadOnlyList<PieChartValue>) values);
    
        /// <summary>
        /// Draws a pie chart. 
        /// </summary>
        /// <param name="rect">The position of the pie chart</param>
        /// <param name="showLabels">Whether or not to draw the labels</param>
        /// <param name="values">The values to display in the chart</param>
        public static void DrawPieChart(Rect rect, bool showLabels, [NotNull] IReadOnlyList<PieChartValue> values)
        {
            float chartSize = rect.height;
            Vector3 pieCenter = new Vector3(chartSize / 2, chartSize / 2, 0) + (Vector3) rect.position;
        
            Handles.BeginGUI();
        
            DrawGraphicPieChart(pieCenter, chartSize, rect.height / 10, values);

            if (showLabels && values.Any(val => val.Label != null))
            {
                float labelMarginFactor = Mathf.InverseLerp(110, 350, rect.height) * 0.20f + 0.15f;
                float labelMargin = labelMarginFactor * rect.height;
            
                float labelX = rect.x + chartSize + labelMargin;
                float labelWidth = rect.width - chartSize;
            
                var labelZoneHeight = (rect.height - 2 * labelMargin);
                var labelZoneY = labelMargin + rect.y;
                IEnumerable<float> labelsY = Enumerable.Range(0, values.Count)
                    .Select(i => labelZoneY + i * labelZoneHeight / values.Count);
            

                int i = 0;
                foreach (var labelY in labelsY)
                {
                    var value = values[i];

                    DrawLabel(value, labelX, labelY, labelWidth);
                    i++;
                }
            }

            Handles.EndGUI();
        }

        private static void DrawGraphicPieChart(Vector3 pieCenter, float chartSize, float maxThickness, [NotNull] IReadOnlyList<PieChartValue> values)
        {
            const float spacingBetweenArcs = 5;
            float totalValue = values.Select(v => v.Value).Sum();
            Vector3 currentPieVector = Vector3.down; // Vector3.down is up in window space
            var drawRadius = (chartSize - maxThickness) / 2;

            foreach (var value in values)
            {
                float angle = value.Value * 360 / totalValue;

                Handles.color = value.Color;

                float drawAngle = angle - spacingBetweenArcs;
                float arcThickness = maxThickness * Mathf.Clamp01(value.Thickness);
                HandlesDrawWireArc(pieCenter, Vector3.forward, currentPieVector, drawAngle, drawRadius, Mathf.Max(arcThickness, 1));

                currentPieVector = Quaternion.AngleAxis(angle, Vector3.forward) * currentPieVector;
            }
        }

        private static void DrawLabel(PieChartValue value, float labelX, float labelY, float width)
        {
            const float colorSquareMargin = 4;
            const float colorSquareSize = 10;
            const float colorLegendSize = colorSquareSize + 2 * colorSquareMargin;

            float colorSquareHeight = Mathf.Max(colorSquareSize * Mathf.Clamp01(value.Thickness), 3);
            Rect colorLegendRect = new Rect(
                labelX + colorSquareMargin, 
                labelY + colorSquareMargin + (colorSquareSize - colorSquareHeight) / 2, 
                colorSquareSize,
                colorSquareHeight
            );
            EditorGUI.DrawRect(colorLegendRect, value.Color);

            Rect labelRect = new Rect(
                labelX + colorLegendSize,
                labelY,
                width - colorLegendSize,
                EditorGUIUtility.singleLineHeight
            );
            GUI.Label(labelRect, value.Label);
        }

        #region DrawWireArc substitute
        private const float SamplePerDegree = 1;

        private static void HandlesDrawWireArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, float thickness)
        {
            // We cannot use Handles.DrawWireArc, since arcs drawn in an EditorWindow are broken.
            // Handles.DrawWireArc(center, normal, from, angle, radius, thickness); return;

            from = from.normalized;
            int sampleCount = Mathf.CeilToInt(angle * SamplePerDegree);

            Vector3 GetVectorFrom(float givenAngle) => Quaternion.AngleAxis(givenAngle, normal) * from;
         
            float currentAngle = 0;

            for (int i = 1; i < sampleCount; i++)
            {
                float nextAngle = (i == sampleCount - 1) ? angle : SamplePerDegree * i;

                // We increase the angles so that there is no ugly lines due to AA with adjacent objects
                Vector3 currentVector = GetVectorFrom(currentAngle - (i == 1 ? 0 : 1));
                Vector3 nextVector = GetVectorFrom(nextAngle + (i == sampleCount - 1 ? 0 : 1));

                Vector3[] polygon = new Vector3[4];
            
                polygon[0] = center + currentVector * (radius + thickness / 2);
                polygon[1] = center + nextVector * (radius + thickness / 2);
                polygon[2] = center + nextVector * (radius - thickness / 2);
                polygon[3] = center + currentVector * (radius - thickness / 2);
            
                Handles.DrawAAConvexPolygon(polygon);

                currentAngle = nextAngle;
            }
        }
        #endregion
    }
}