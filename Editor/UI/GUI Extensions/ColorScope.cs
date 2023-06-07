using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class EditorGUIExtension
    {
        public class ColorScope : GUI.Scope
        {
            private readonly Color PreviousColor;
            private readonly Color PreviousContentColor;
            
            public ColorScope(Color color)
            {
                PreviousColor = GUI.color;
                PreviousContentColor = GUI.contentColor;
                
                GUI.color = color;
                GUI.contentColor = color;
            }
            
            protected override void CloseScope()
            {
                GUI.color = PreviousColor;
                GUI.contentColor = PreviousContentColor;
            }
        }
    }
}