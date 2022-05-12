using System;
using System.Collections.Generic;
using UnityEditor;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Utility to find all implemented Checks in the project.
    /// </summary>
    public static class AvailableChecks
    {
        private static Dictionary<string, List<ICheck>> _registeredChecks = new Dictionary<string, List<ICheck>>();

        [InitializeOnLoadMethod]
        public static void OnAssemblyReload()
        {
            var checkTypes = TypeCache.GetTypesDerivedFrom<ICheck>();
            foreach (var check in checkTypes)
            {
                if (check.IsInterface || check.IsAbstract) continue;
                var checkInstance = Activator.CreateInstance(check);
                ICheck typedCheckInstance = checkInstance as ICheck;
                if (typedCheckInstance == null) continue;
                foreach (var tag in typedCheckInstance.Tags)
                {
                    if (!_registeredChecks.ContainsKey(tag))
                        _registeredChecks.Add(tag, new List<ICheck>());
                    _registeredChecks[tag].Add(typedCheckInstance);
                }
            }
        }

        public static List<ICheck> GetAllChecksWithTag(string tag)
        {
            return _registeredChecks.TryGetValue(tag, out var list) ? list : new List<ICheck>();
        }
    }
}