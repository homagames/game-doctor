using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public static partial class EditorGUILayoutExtension
    {
        [NotNull] 
        private static readonly Stack<Rect> CurrentDrawRect = new Stack<Rect>();

        private const float SeparatedViewSeparatorWidth = 2;
    
        /// <summary>
        /// Call this method to draw a rectangle split in two.
        /// Users will be able to move the separator.<br /><br />
        /// Once you have called this method, you will start drawing on the first pane of the view.
        /// To switch to the other, call <see cref="PutSeparatorInView"/>.
        /// <br /><br />
        /// To close the rectangle after that, call <see cref="EndSeparatedView"/>
        /// </summary>
        /// <param name="height">The height of the view</param>
        /// <param name="state">The current state of the view. The return value of this function should be assigned back to this parameter.</param>
        /// <param name="options">An optional list of layout options that specify extra layouting properties. Any values passed in here will override settings defined by the style.<br /> See Also: GUILayout.Width, GUILayout.Height, GUILayout.MinWidth, GUILayout.MaxWidth, GUILayout.MinHeight, GUILayout.MaxHeight, GUILayout.ExpandWidth, GUILayout.ExpandHeight.</param>
        /// <returns>The new view state.</returns>
        public static SeparatedViewData BeginSeparatedView(float height, SeparatedViewData state, [NotNull] params GUILayoutOption[] options)
        {
            InitSeparatorPosition(ref state);
        
            Rect guiRect = GUILayoutUtility.GetRect(EditorGUIUtility.fieldWidth, height, options);
            if (Event.current.type == EventType.Repaint)
            {
                state.DrawRect = guiRect;
            }
            var drawRect = state.DrawRect;
            drawRect.height = height;
        
            GUILayout.BeginArea(new Rect(drawRect) {width = state.SeparatorPosition});
            CurrentDrawRect.Push(drawRect);
        
            return state;
        }

        /// <summary>
        /// Puts the separator in the separated view. Starts the drawing for the second view.
        /// </summary>
        /// <param name="state">The current state of the view. The return value of this function should be assigned back to this parameter</param>
        /// <param name="minWidthLeft">The minimum width for the left panel</param>
        /// <param name="minWidthRight">The minimum width for the right panel</param>
        /// <returns>The new view state.</returns>
        /// <seealso cref="BeginSeparatedView"/>
        public static SeparatedViewData PutSeparatorInView(SeparatedViewData state, float minWidthLeft = 1, float minWidthRight = 1)
        {
            InitSeparatorPosition(ref state);
        
            if (CurrentDrawRect.Count == 0)
            {
                Debug.LogError("GUI Error: Invalid GUILayout state in view. Verify that all layout Begin/PutSeparator/End calls match");
                return state;
            }
            var drawRect = CurrentDrawRect.Pop();


            GUILayout.EndArea();

            var separatorRect = DrawSeparator(state, drawRect);
            state = HandleSeparatorDragging(state, drawRect, separatorRect, minWidthLeft, minWidthRight);
        
            GUILayout.BeginArea(new Rect(drawRect)
            {
                x = drawRect.x + state.SeparatorPosition,
                width = drawRect.width - state.SeparatorPosition
            });
            return state;
        }
    
        /// <summary>
        /// Ends the second panel of a separated view.
        /// </summary>
        /// <seealso cref="BeginSeparatedView"/>
        public static void EndSeparatedView()
        {
            GUILayout.EndArea();
        }
    

        private static void InitSeparatorPosition(ref SeparatedViewData state)
        {
            if (!state.IsInitialized && Event.current.type == EventType.Repaint)
            {
                state.IsInitialized = true;
                state.SeparatorPosition = EditorGUIUtility.currentViewWidth / 2;
            }
        }
    
        private static SeparatedViewData HandleSeparatorDragging(SeparatedViewData state, Rect drawRect, Rect separatorRect,
            float minWidthLeft, float minWidthRight)
        {
            if (Event.current.type == EventType.MouseDown && separatorRect.Contains(Event.current.mousePosition))
                state.IsResizing = true;

            if (Event.current.type == EventType.MouseUp)
                state.IsResizing = false;


            var isUserResizing = state.IsResizing && Event.current.type == EventType.MouseDrag;
            bool isViewLargeEnough = drawRect.width > minWidthLeft + minWidthRight;
            if (isViewLargeEnough)
            {
                if (isUserResizing)
                {
                    state.SeparatorPosition = Event.current.mousePosition.x;
                    GUI.changed = true;
                }

                if (Event.current.type == EventType.Repaint || isUserResizing)
                {
                    float newSeparatorPosition = Mathf.Clamp(state.SeparatorPosition, minWidthLeft,
                        drawRect.width - minWidthRight);

                    if (Math.Abs(newSeparatorPosition - state.SeparatorPosition) > 0.05f)
                    {
                        state.SeparatorPosition = newSeparatorPosition;
                        GUI.changed = true;
                    }
                }
            }
            else
            {
                state.SeparatorPosition = drawRect.width / 2;
            }

            return state;
        }

        private static Rect DrawSeparator(SeparatedViewData state, Rect drawRect)
        {
            Rect separatorRect = EditorGUIExtension.DrawVerticalSeparator(
                new Vector2(
                    state.SeparatorPosition + drawRect.x,
                    drawRect.y),
                drawRect.height,
                SeparatedViewSeparatorWidth);

            EditorGUIUtility.AddCursorRect(separatorRect, MouseCursor.SplitResizeLeftRight);
            return separatorRect;
        }
    }

    public struct SeparatedViewData
    {
        public Rect DrawRect;
        public float SeparatorPosition;
        public bool IsResizing;

        public bool IsInitialized;
    }
}