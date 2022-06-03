using System.Collections.Generic;
using JetBrains.Annotations;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Utility to get all registered Validation Profiles in the project.
    /// </summary>
    public static class AvailableProfiles
    {
        private static readonly Dictionary<string, IValidationProfile> _registeredValidationProfiles =
            new Dictionary<string, IValidationProfile>();

        /// <summary>
        /// Register a Validation Profile to make it publicly available in the Game Doctor UI.
        /// </summary>
        /// <param name="validationProfile">The Validation Profile to be added</param>
        [PublicAPI]
        public static void RegisterValidationProfile(IValidationProfile validationProfile)
        {
            if (_registeredValidationProfiles.ContainsKey(validationProfile.Name)) return;
            _registeredValidationProfiles.Add(validationProfile.Name,validationProfile);
        }

        /// <summary>
        /// Try to find a registered Validation Profile with a specific name.
        /// </summary>
        /// <param name="validationProfileName">Name of the profile</param>
        /// <param name="validationProfile">The profile with the corresponding name</param>
        [PublicAPI]
        public static bool TryGetValidationProfileByName(string validationProfileName, out IValidationProfile validationProfile)
        {
            return _registeredValidationProfiles.TryGetValue(validationProfileName, out validationProfile);
        }
        

        /// <summary>
        /// Get all available Validation Profiles.
        /// </summary>
        [PublicAPI]
        public static List<IValidationProfile> GetAllValidationProfiles()
        {
            return new List<IValidationProfile>(_registeredValidationProfiles.Values);
        }

        /// <summary>
        /// Gets the default Validation Profile to be used when no other validation profiles are available.
        /// </summary>
        [PublicAPI]
        public static IValidationProfile GetDefaultValidationProfile()
        {
            if (TryGetValidationProfileByName(DefaultValidationProfile.DefaultValidationProfileName, out IValidationProfile validationProfile))
                return validationProfile;
            DefaultValidationProfile defaultValidationProfile = new DefaultValidationProfile();
            RegisterValidationProfile(defaultValidationProfile);
            return defaultValidationProfile;
        }
    }
}