using System.Collections.Generic;
using System.Linq;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Stores information of a check execution.
    /// </summary>
    public class CheckResult
    {
        public bool Passed => Issues.All(issue => issue.HasBeenDismissed());
        public List<IIssue> Issues = new List<IIssue>();
    }
}