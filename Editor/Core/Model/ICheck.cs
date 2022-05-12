using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public interface ICheck
    {
        /// <summary>
        /// Called whenever the CheckResult is generated again.
        /// </summary>
        event System.Action<ICheck> OnExecuted;
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
        List<string> Tags { get; }
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