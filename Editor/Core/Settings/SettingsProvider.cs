using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor
{
    public static class GameDoctorSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new SettingsProvider("Project/MyCustomIMGUISettings", SettingsScope.Project)
            {
                label = "Game Doctor",
                guiHandler = (searchContext) =>
                {
                    SettingsDrawer.Draw();
                },

                keywords = new HashSet<string>(new[] { "doctor", "homa", "game" })
            };

            return provider;
        }
    }
}