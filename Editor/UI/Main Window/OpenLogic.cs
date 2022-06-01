using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    public partial class SplitViewWindow
    {
        private struct TextureToLoad
        {
            public string Name;
            public Action<SplitViewWindow, Texture2D> Setter;

            public void Load(SplitViewWindow window, string textureFolderPath)
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
        
        [MenuItem("Test/Split View")]
        public static void Open()
        {
            string iconFolderPath = FolderLocator.GetFolder("GameDoctor.Icon");

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
                    Name = "FixedIcon",
                    Setter = (window, texture) => window.FixedTexture = texture
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
            };
            
            var window = GetWindow<SplitViewWindow>();

            foreach (var icon in iconsToLoad)
            {
                icon.Load(window, iconFolderPath);
            }
            
            
            window.Show();
        }

    }
}