using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public interface IValidationProfile
    {
        [PublicAPI]
        event System.Action OnBeforeCheck;
        [PublicAPI]
        event System.Action OnAfterCheck;
        [PublicAPI]
        event System.Action<ICheck> OnCheckExecuted;
        [PublicAPI]
        event System.Action<IIssue> OnFixExecuted;
        string Name { get; }
        string Description { get; }
        List<ICheck> CheckList { get; }
        Task Check();
    }
}