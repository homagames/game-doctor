using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Utility to get all registered Checks in the project.
    /// </summary>
    public static class AvailableChecks
    {
        private static readonly HashSet<ICheck> _allregisteredChecks = new HashSet<ICheck>();
        private static readonly Dictionary<string, List<ICheck>> _registeredChecks = new Dictionary<string, List<ICheck>>();
        
        /// <summary>
        /// Register a check to make it publicly available to all Validation Profiles.
        /// </summary>
        /// <param name="check">The check to be added</param>
        [PublicAPI]
        public static void RegisterCheck(ICheck check)
        {
            if (_allregisteredChecks.Contains(check)) return;
            _allregisteredChecks.Add(check);
            foreach (var tag in check.Tags)
            {
                if (!_registeredChecks.ContainsKey(tag))
                    _registeredChecks.Add(tag, new List<ICheck>());
                if(!_registeredChecks[tag].Exists(c=>c.Same(check)))
                    _registeredChecks[tag].Add(check);
            }
        }

        /// <summary>
        /// Get the list of all available checks having this tag
        /// </summary>
        [PublicAPI]
        public static List<ICheck> GetAllChecksWithTag(string tag)
        {
            return _registeredChecks.TryGetValue(tag, out var list) ? list : new List<ICheck>();
        }
        
        /// <summary>
        /// Get a list of all available checks.
        /// </summary>
        [PublicAPI]
        public static List<ICheck> GetAllChecks()
        {
            return new List<ICheck>(_allregisteredChecks);
        }
        
        /// <summary>
        /// Get the union of all tags from all registered checks.
        /// </summary>
        [PublicAPI]
        public static List<string> GetAllTags()
        {
            return new List<string>(_registeredChecks.Keys);
        }
    }
}