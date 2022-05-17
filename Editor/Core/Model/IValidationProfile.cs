using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public interface IValidationProfile
    {
        /// <summary>
        /// The name of the Validation Profile
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The description for this validation profile.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The list of checks this profile contains.
        /// </summary>
        List<ICheck> CheckList { get; }
        /// <summary>
        /// Populates CheckResults for all Checks in the CheckList.
        /// </summary>
        Task Check();
    }
}