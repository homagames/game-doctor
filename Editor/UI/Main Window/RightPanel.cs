using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class SplitViewWindow
    {
        private void DrawRightPanel()
        {
            SecondViewScroll = EditorGUILayout.BeginScrollView(SecondViewScroll);
            GUILayout.Label("Description");
            EditorGUILayout.EndScrollView();
        }
    }
}
