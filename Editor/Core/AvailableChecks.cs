using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Utility to find all implemented Checks in the project.
    /// </summary>
    public static class AvailableChecks
    {
        private static Dictionary<string, List<ICheck>> _registeredChecks = new Dictionary<string, List<ICheck>>();

        [PublicAPI]
        public static void RegisterCheck(ICheck check)
        {
            foreach (var tag in check.Tags)
            {
                if (!_registeredChecks.ContainsKey(tag))
                    _registeredChecks.Add(tag, new List<ICheck>());
                if(!_registeredChecks[tag].Exists(c=>c.Same(check)))
                    _registeredChecks[tag].Add(check);
            }
        }

        [PublicAPI]
        public static List<ICheck> GetAllChecksWithTag(string tag)
        {
            return _registeredChecks.TryGetValue(tag, out var list) ? list : new List<ICheck>();
        }
    }
}