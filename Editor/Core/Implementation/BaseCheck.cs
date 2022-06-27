using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public abstract class BaseCheck : ICheck
    {
        protected BaseCheck(string name, string description,
            HashSet<string> tags, ImportanceType importance = ImportanceType.Advised,
            Priority priority = Priority.Medium)
        {
            Importance = importance;
            Priority = priority;
            Name = name;
            Description = description;
            Tags = tags;
        }

        public event Action<ICheck, IIssue, bool> OnIssueFixExecuted;
        public event Action<ICheck> OnResultGenerated;
        public string Name { get; }
        public string Description { get; }
        public HashSet<string> Tags { get; }
        public ImportanceType Importance { get; }
        public Priority Priority { get; }
        protected abstract Task<CheckResult> GenerateCheckResult();

        public async Task Execute()
        {
            CheckResult = await GenerateCheckResult();
            foreach (var issue in CheckResult.Issues)
            {
                issue.OnFixExecuted += (i, actuallyFixed) => OnIssueFixExecuted?.Invoke(this, i, actuallyFixed);
            }

            OnResultGenerated?.Invoke(this);
        }

        public CheckResult CheckResult { get; private set; }
    }
}