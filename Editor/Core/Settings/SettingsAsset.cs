using System.Collections.Generic;
using UnityEditor;

namespace HomaGames.GameDoctor.Core
{
    [FilePath(SettingsAsset.SettingsPath, FilePathAttribute.Location.ProjectFolder)]
    public class SettingsAsset : ScriptableSingleton<SettingsAsset>
    {
        public const string SettingsPath = "ProjectSettings/GameDoctorSettings.asset";

        [InitializeOnLoadMethod]
        private static void RegisterAllProfiles()
        {
            foreach (var profile in instance.ProfileList)
                AvailableProfiles.RegisterValidationProfile(profile);
        }

        public List<TagBasedValidationProfile> ProfileList = new List<TagBasedValidationProfile>();
        
        public void Save()
        {
            Save(true);
        }
        
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Project/Game Doctor", SettingsScope.Project)
            {
                label = "Game Doctor",
                guiHandler = (searchContext) =>
                {
                    SettingsDrawer.Draw();
                },

                keywords = new HashSet<string>(new[] { "doctor", "game" , "tests"})
            };

            return provider;
        }
    }
}