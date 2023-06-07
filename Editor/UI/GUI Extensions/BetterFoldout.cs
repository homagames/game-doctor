using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class EditorGUIExtension
    {
        /// <summary>
        /// <see cref="EditorGUI.Foldout(Rect, bool, GUIContent, bool, GUIStyle)">EditorGUI.Foldout</see> has a major flaw:
        /// its width is fixed to <see cref="EditorGUIUtility.labelWidth"/>. To fix that, we decompiled from Unity 2020.3 and edited
        /// the method, ot better fit our needs.
        /// </summary>
        /// <param name="position">Rectangle on the screen to use for the arrow and label.</param>
        /// <param name="foldout">The shown foldout state.</param>
        /// <param name="content">The label to show.</param>
        /// <param name="style">Optional GUIStyle.</param>
        /// <param name="toggleOnLabelClick">Should the label be a clickable part of the control?</param>
        /// <returns></returns>
        public static bool BetterFoldout(
            Rect position,
            bool foldout,
            GUIContent content,
            bool toggleOnLabelClick,
            GUIStyle style)
        {
            Rect rect1 = position;
            position = style.margin.Remove(position);
            if (EditorGUIUtility.hierarchyMode)
            {
                int num = EditorStyles.foldout.padding.left - EditorStyles.label.padding.left;
                position.xMin -= num;
            }
            int controlId = GUIUtility.GetControlID(FocusType.Keyboard, position);
            EventType eventType = Event.current.type;
            if (!GUI.enabled && (Event.current.rawType == EventType.MouseDown || Event.current.rawType == EventType.MouseDrag || Event.current.rawType == EventType.MouseUp))
                eventType = Event.current.rawType;
            Rect rect2 = position;
            if (!toggleOnLabelClick)
            {
                rect2.width = style.padding.left;
                rect2.x += EditorGUI.indentLevel * 16;
            }
            switch (eventType)
            {
                case EventType.MouseDown:
                    if (rect2.Contains(Event.current.mousePosition) && Event.current.button == 0)
                    {
                        GUIUtility.keyboardControl = GUIUtility.hotControl = controlId;
                        Event.current.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlId)
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                        if (rect2.Contains(Event.current.mousePosition))
                        {
                            GUI.changed = true;
                            return !foldout;
                        }
                    }
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlId)
                    {
                        Event.current.Use();
                    }
                    break;
                case EventType.KeyDown:
                    if (GUIUtility.keyboardControl == controlId)
                    {
                        KeyCode keyCode = Event.current.keyCode;
                        if (keyCode == KeyCode.LeftArrow & foldout || keyCode == KeyCode.RightArrow && !foldout)
                        {
                            foldout = !foldout;
                            GUI.changed = true;
                            Event.current.Use();
                        }
                    }
                    break;
                case EventType.Repaint:
                    // [EDITED]: changed the width of the rects during repaint
                    // EditorStyles.foldout.Draw(position, GUIContent.none, controlId, false);
                    // Rect position1 = new Rect(position.x + EditorGUI.indentLevel * 16, position.y, EditorGUIUtility.labelWidth - EditorGUI.indentLevel * 16, position.height);
                    Rect position1 = new Rect(position.x + EditorGUI.indentLevel * 16, position.y, position.width - EditorGUI.indentLevel * 16, position.height);
              
                    if (EditorGUI.showMixedValue && !foldout)
                    {
                        style.Draw(position1, content, controlId, false);
                        Rect position2 = rect1;
                        position2.xMin += EditorGUIUtility.labelWidth;
                        EditorStyles.label.Draw(position2, content, controlId, false);
                        break;
                    }
                    style.Draw(position1, content, controlId, foldout);
                    break;
                // [EDITED]: removed all the dragging related events. It is not relevant here, and methods were internal anyway.
            }
            return foldout;
        }
    }
}
