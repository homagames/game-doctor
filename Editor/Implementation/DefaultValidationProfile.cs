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

        public string Description =>
            "Default Validation Profile with all available checks.\n\n" +
            "Checkout Project Settings > Game Doctor and create new Profile from presets.\n" +
            "Implement checks by inheriting ICheck and registering them with AvailableChecks.RegisterCheck.";

        public HashSet<ICheck> CheckList => new HashSet<ICheck>(AvailableChecks.GetAllChecks());
    }
}