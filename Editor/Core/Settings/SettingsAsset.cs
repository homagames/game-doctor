using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor
{
    public class SettingsAsset : ScriptableObject
    {
        public const string SettingsPath = "Assets/GameDoctorSettings.asset";

        public static SettingsAsset Get
        {
            get
            {
                var asset = AssetDatabase.LoadAssetAtPath<SettingsAsset>(SettingsPath);
                if (asset) return asset;
                var newInstance = CreateInstance<SettingsAsset>();
                AssetDatabase.CreateAsset(newInstance, SettingsPath);
                EditorApplication.delayCall += AssetDatabase.SaveAssets;

                return newInstance;
            }
        }

        [InitializeOnLoadMethod]
        private static void RegisterAllProfiles()
        {
            foreach (var profile in Get.ProfileList)
                AvailableProfiles.RegisterValidationProfile(profile);
        }

        public List<TagBasedValidationProfile> ProfileList;
    }
}