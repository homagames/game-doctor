using System.Collections.Generic;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Default Validation Profile when no other implementations are available.
    /// </summary>
    public sealed class DefaultValidationProfile : IValidationProfile
    {
        public const string DefaultValidationProfileName = "Default Validation Profile";
        public string Name => DefaultValidationProfileName;
        public string Description => "Default Validation Profile with all available checks.";
        public HashSet<ICheck> CheckList => new HashSet<ICheck>(AvailableChecks.GetAllChecks());
    }
}