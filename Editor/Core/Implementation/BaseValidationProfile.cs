using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public class BaseValidationProfile : IValidationProfile
    {
        public event Action OnBeforeCheck;
        public event Action OnAfterCheck;
        public event Action<ICheck> OnCheckExecuted
        {
            add
            {
                foreach (var check in CheckList)
                {
                    check.OnExecuted -= value;
                    check.OnExecuted += value;
                }
            }
            remove
            {
                foreach (var check in CheckList)
                {
                    check.OnExecuted -= value;
                }
            }
        }
        public event Action<IIssue> OnFixExecuted;
        public string Name { get; }
        public List<ICheck> CheckList { get; } = new List<ICheck>();
        public string Description { get; }

        public BaseValidationProfile(string name, string description)
        {
            Name = name;
            Description = description;
        }
        
        public async Task Check()
        {
            OnBeforeCheck?.Invoke();
            foreach (var check in CheckList)
                await check.Execute();
            OnAfterCheck?.Invoke();
        }
    }
}