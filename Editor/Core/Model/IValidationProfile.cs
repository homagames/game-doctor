using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public interface IValidationProfile
    {
        event System.Action OnBeforeExecute;
        event System.Action OnAfterExecute;
        event System.Action<ICheck> OnCheckExecuted;
        event System.Action<IIssue> OnFixExecuted;
        string Name { get; }
        string Description { get; }
        List<ICheck> CheckList { get; }
        Task Check();
        Task Fix();
        Task CheckAndFix();
    }
}