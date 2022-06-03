using UnityEditor;

namespace HomaGames.GameDoctor
{
    [CustomEditor(typeof(SettingsAsset))]
    public class SettingsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            SettingsDrawer.Draw();
        }
    }
}