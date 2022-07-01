using HomaGames.GameDoctor.Core;
using UnityEditor;

namespace HomaGames.GameDoctor.Checks
{
    /// <summary>
    /// Class in charge of registering all default checks
    /// </summary>
    public static class ChecksRegistration
    {
        [InitializeOnLoadMethod]
        private static void RegisterDefaultChecks()
        {
            AvailableChecks.RegisterCheck(new PackageManagerCheck());
            AvailableChecks.RegisterCheck(new PlayerSettingsCheck());
        }
    }
}