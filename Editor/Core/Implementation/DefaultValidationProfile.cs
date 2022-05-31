using UnityEditor;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Default Validation Profile when no other implementations are available.
    /// </summary>
    public class DefaultValidationProfile : BaseValidationProfile
    {
        public DefaultValidationProfile() : base("Default Validation Profile",
            "Default Validation Profile with selectable tags.")
        {
            
        }
    }
}