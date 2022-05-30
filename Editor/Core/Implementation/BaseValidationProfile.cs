using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public class BaseValidationProfile : IValidationProfile
    {
        public string Name { get; }
        public List<ICheck> CheckList { get; } = new List<ICheck>();
        public string Description { get; }

        public BaseValidationProfile(string name, string description)
        {
            Name = name;
            Description = description;
        }
        
        public virtual async Task Check()
        {
            foreach (var check in CheckList)
                await check.Execute();
        }
    }
}