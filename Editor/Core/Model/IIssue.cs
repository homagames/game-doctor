using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public interface IIssue
    {
        /// <summary>
        /// When the fix is executed.
        /// </summary>
        event System.Action<IIssue> OnExecuted;
        /// <summary>
        /// Unique name for the issue.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Short description of the issue, this should be displayed by default in the issue inspector.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// The inspector drawing function.
        /// </summary>
        void Draw();
        /// <summary>
        /// The originated check for this issue.
        /// </summary>
        ICheck Check { get; }
        AutomatableType AutomatableType { get; }
        Priority Priority { get; }
        /// <summary>
        /// Launches the fixing process, either automatic or interactive.
        /// </summary>
        Task Fix();
    }
}