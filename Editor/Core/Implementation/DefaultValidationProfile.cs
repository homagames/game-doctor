using UnityEditor;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Default Validation Profile when no other implementations are available.
    /// </summary>
    public class DefaultValidationProfile : BaseValidationProfile
    {
        public const string DefaultValidationProfileName = "Default Validation Profile";
        public DefaultValidationProfile() : base(DefaultValidationProfileName,
            "Default Validation Profile with selectable tags.")
        {
            
        }
    }
}