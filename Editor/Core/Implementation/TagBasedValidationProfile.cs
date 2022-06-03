using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    [Serializable]
    public class TagBasedValidationProfile : IValidationProfile
    {
        [SerializeField]
        private string _name;
        [SerializeField]
        private string _description;
            
        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string Description
        {
            get => _description;
            set => _description = value;
        }

        public List<string> Tags;
        public HashSet<ICheck> CheckList => new HashSet<ICheck>(Tags.SelectMany(AvailableChecks.GetAllChecksWithTag));
    }
}