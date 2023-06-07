using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Defines a way to validate a specific part of your Unity Project.
    /// An ICheck can generate a CheckResult containing (or not) a list of issues.
    /// </summary>
    public interface ICheck
    {
        /// <summary>
        /// Triggered when an issue is actually fixed.
        /// Returns true if the issue process went through.
        /// </summary>
        [PublicAPI]
        event System.Action<ICheck,IIssue,bool> OnIssueFixExecuted;
        /// <summary>
        /// Called whenever the CheckResult is generated again.
        /// </summary>
        [PublicAPI]
        event System.Action<ICheck> OnResultGenerated;
        /// <summary>
        /// Unique name for the check.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Short description for this check.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Tags identifying the check. Checks can be found project-wide using the tag system.
        /// </summary>
        HashSet<string> Tags { get; }
        ImportanceType Importance { get; }
        Priority Priority { get; }
        /// <summary>
        /// Populates the CheckResult.
        /// </summary>
        Task Execute();
        /// <summary>
        /// The result from the last Execution of the check.
        /// </summary>
        CheckResult CheckResult { get; }
    }
}