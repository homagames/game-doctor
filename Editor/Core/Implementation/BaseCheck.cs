using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public abstract class BaseCheck : ICheck
    {
        public event Action<ICheck> OnExecuted;
        public string Name { get; }
        public string Description { get; }
        public abstract List<string> Tags { get; }
        public ImportanceType Importance { get; }
        public Priority Priority { get; }
        protected abstract Task<CheckResult> GenerateCheckResult();

        public async Task Execute()
        {
            CheckResult = await GenerateCheckResult();
            OnExecuted?.Invoke(this);
        }

        public CheckResult CheckResult { get; private set; }
    }
}