using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    /// <summary>
    /// Stores information of a check execution.
    /// </summary>
    public class CheckResult
    {
        public bool Passed => Issues.Count == 0;
        public List<IIssue> Issues = new List<IIssue>();
    }
}