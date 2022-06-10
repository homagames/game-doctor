using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor
{
    public static class AssetDatabaseUtilities
    {
        public static List<T> GetAll<T>(string[] searchInFolders = null) where T : Object {
            
            List<T> output = new List<T>();
            
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, searchInFolders);
            for (var i = 0; i < guids.Length; i++) {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                var element = AssetDatabase.LoadAssetAtPath<T>(path);
                if (element) {
                    output.Add(element);
                }
            }
          
            return output;
        }
    }
}