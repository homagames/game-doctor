using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HomaGames.GameDoctor
{
    public class ScriptableValidationProfile : ValidationProfile
    {
        private ValidationProfileConfiguration validationProfileConfiguration;

        public ScriptableValidationProfile(ValidationProfileConfiguration config)
        {
            validationProfileConfiguration = config;
        }

        public override List<Check> Checks => validationProfileConfiguration.Checks;
    }
}