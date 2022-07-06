using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Ui
{
    /// <summary>
    /// These assets allow you to easily locate folders throughout the project.
    /// Place one in a folder you want to have the path to, assign it a folder ID,
    /// and then call <see cref="FolderLocator.GetFolder">FolderLocator.GetFolder</see>
    /// with the said ID to get the folder path directly.
    /// <br /><br />
    /// This allows for seamless project folder sorting.   
    /// </summary>
    public class FolderLocator : ScriptableObject {

        public string FolderId;

        /// <summary>
        /// Tries to find the <see cref="FolderLocator"/> with the given ID and return the folder path
        /// </summary>
        /// <param name="id">The ID of the <see cref="FolderLocator"/> asset to look for</param>
        /// <returns>The path of the <see cref="FolderLocator"/>'s folder, or null if no <see cref="FolderLocator"/> has been found</returns>
        [CanBeNull]
        public static string GetFolder(string id) {

            List<FolderLocator> locators = AssetDatabaseUtilities.GetAll<FolderLocator>();

            foreach (var folderLocator in locators) {
                if (folderLocator.FolderId == id) {
                    return Path.GetDirectoryName(AssetDatabase.GetAssetPath(folderLocator));
                }
            }

            return null;
        }
    }
}
