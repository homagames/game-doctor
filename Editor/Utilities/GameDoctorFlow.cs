using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HomaGames.GameDoctor.Utilities
{
    /// <summary>
    /// Utility to execute actions outside of an Issue context
    /// </summary>
    public static class GameDoctorFlow
    {
        private static readonly HashSet<Action> _onPostFixAction = new HashSet<Action>();
        
        /// <summary>
        /// Register to this event to execute an action after a series of Automated fixes are executed
        /// </summary>
        public static event Action OnPostFixAction
        {
            add => _onPostFixAction.Add(value);
            remove => _onPostFixAction.Remove(value);
        }
        /// <summary>
        /// Register an asset importer to reimport after all fixes are executed on it
        /// </summary>
        /// <param name="assetImporter"></param>
        public static void RegisterForReimport(AssetImporter assetImporter)
        {
            OnPostFixAction += assetImporter.SaveAndReimport;
        }

        public static int PostFixActionCount => _onPostFixAction.Count;

        public static bool ExecuteNextPostFixAction()
        {
            if (_onPostFixAction.Count <= 0) return false;
            var action = _onPostFixAction.First();
            action();
            _onPostFixAction.Remove(action);
            return true;
        }
    }
}