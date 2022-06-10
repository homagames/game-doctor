using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Defines a collection of ICheck.
    /// This is convenient to groups checks together and validate them all together.
    /// </summary>
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
        HashSet<ICheck> CheckList { get; }
    }
}