using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    [Serializable]
    public class TagBasedValidationProfile : IValidationProfile
    {
        [SerializeField] private string _name;
        [SerializeField] private string _description;

        public TagBasedValidationProfile()
        {
        }

        public TagBasedValidationProfile(string name, string description)
        {
            _name = name;
            _description = description;
        }

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

        /// <summary>
        /// The CheckList will be filled with all available Checks having those tags.
        /// </summary>
        public List<string> Tags = new List<string>();

        public HashSet<ICheck> CheckList => new HashSet<ICheck>(Tags.SelectMany(AvailableChecks.GetAllChecksWithTag));
    }
}