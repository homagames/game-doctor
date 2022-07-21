using System;
using System.Collections.Generic;
using System.IO;
using HomaGames.GameDoctor.Core;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class GameDoctorWindow
    {
        private const string IconFolderId = "GameDoctor.Icon";

        private struct TextureToLoad
        {
            [NotNull]
            public string Name;
            [NotNull]
            public Action<GameDoctorWindow, Texture2D> Setter;

            public void Load([NotNull] GameDoctorWindow window, [NotNull] string textureFolderPath)
            {
                Texture2D texture;
                string regularPath = Path.Combine(textureFolderPath, Name + ".png");
                string proPath = Path.Combine(textureFolderPath, Name + "Pro.png");

                if (EditorGUIUtility.isProSkin && File.Exists(proPath))
                    texture = AssetDatabase.LoadAssetAtPath<Texture2D>(proPath);
                else
                    texture = AssetDatabase.LoadAssetAtPath<Texture2D>(regularPath);
                
                Setter.Invoke(window, texture);
            }
        }
        
        public static void Open(IValidationProfile validationProfile = null)
        {
            string iconFolderPath = FolderLocator.GetFolder(IconFolderId);
            if (iconFolderPath == null)
                throw new DirectoryNotFoundException("[Game Doctor] Icon Directory not found. Could not find folder" +
                                                     $"locator with ID: {IconFolderId}");

            List<TextureToLoad> iconsToLoad = new List<TextureToLoad>
            {
                new TextureToLoad
                {
                    Name = "AutomaticIcon",
                    Setter = (window, texture) => window.AutomaticTexture = texture
                },
                new TextureToLoad
                {
                    Name = "InteractiveIcon",
                    Setter = (window, texture) => window.InteractiveTexture = texture
                },
                new TextureToLoad
                {
                    Name = "FixedIconColored",
                    Setter = (window, texture) => window.FixedColoredTexture = texture
                },
                new TextureToLoad
                {
                    Name = "FixedIconWhite",
                    Setter = (window, texture) => window.FixedWhiteTexture = texture
                },
                new TextureToLoad
                {
                    Name = "MandatoryIcon",
                    Setter = (window, texture) => window.MandatoryTexture = texture
                },
                new TextureToLoad
                {
                    Name = "HighPriorityIcon",
                    Setter = (window, texture) => window.HighPriorityTexture = texture
                },
                new TextureToLoad
                {
                    Name = "MediumPriorityIcon",
                    Setter = (window, texture) => window.MediumPriorityTexture = texture
                },
                new TextureToLoad
                {
                    Name = "LowPriorityIcon",
                    Setter = (window, texture) => window.LowPriorityTexture = texture
                },
                new TextureToLoad
                {
                    Name = "HighPriorityIconWhite",
                    Setter = (window, texture) => window.HighPriorityWhiteTexture = texture
                },
                new TextureToLoad
                {
                    Name = "MediumPriorityIconWhite",
                    Setter = (window, texture) => window.MediumPriorityWhiteTexture = texture
                },
                new TextureToLoad
                {
                    Name = "LowPriorityIconWhite",
                    Setter = (window, texture) => window.LowPriorityWhiteTexture = texture
                },
                new TextureToLoad
                {
                    Name = "ProfileIcon",
                    Setter = (window, texture) => window.ProfileTexture = texture
                },
                new TextureToLoad
                {
                    Name = "CheckIcon",
                    Setter = (window, texture) => window.CheckTexture = texture
                },
                new TextureToLoad
                {
                    Name = "GameDoctorLogoIcon",
                    Setter = (window, texture) => window.GameDoctorLogoTexture = texture
                },
                new TextureToLoad
                {
                    Name = "HiddenIcon",
                    Setter = (window, texture) => window.DismissedTexture = texture
                },
            };
            
            var window = GetWindow<GameDoctorWindow>();

            foreach (var icon in iconsToLoad)
            {
                icon.Load(window, iconFolderPath);
            }


            window.OnTexturesLoaded();
            window.Profile = validationProfile ?? AvailableProfiles.GetDefaultValidationProfile();
            
            
            window.Show();
        }

    }
}