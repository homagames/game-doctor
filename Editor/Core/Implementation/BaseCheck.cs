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
            List<string> tags, ImportanceType importance = ImportanceType.Advised, Priority priority = Priority.Medium)
        {
            Importance = importance;
            Priority = priority;
            Name = name;
            Description = description;
            Tags = tags;
        }

        public event Action<IIssue> OnIssueFixed;
        public event Action<ICheck> OnResultGenerated;
        public string Name { get; }
        public string Description { get; }
        public List<string> Tags { get; }
        public ImportanceType Importance { get; }
        public Priority Priority { get; }
        protected abstract Task<CheckResult> GenerateCheckResult();

        public async Task Execute()
        {
            List<IIssue> previousIssues = CheckResult?.Issues;
            CheckResult = await GenerateCheckResult();
            if (previousIssues != null)
            {
                previousIssues.RemoveAll(i => CheckResult.Issues.Any(i.Same));
                foreach (var fixedIssue in previousIssues)
                {
                    OnIssueFixed?.Invoke(fixedIssue);
                }
            }

            OnResultGenerated?.Invoke(this);
        }

        public CheckResult CheckResult { get; private set; }
    }
}