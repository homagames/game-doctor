using System.Linq;
using UnityEditor;

namespace HomaGames.GameDoctor.Core.Drawers
{
    [CustomEditor(typeof(TagValidationProfile), true)]
    public class TagValidationProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            TagValidationProfile validationProfile = target as TagValidationProfile;
            if (!validationProfile) return;
            
            foreach (var check in validationProfile.CheckList)
            {
                EditorGUILayout.LabelField(check.Name);
            }
        }
    }
}