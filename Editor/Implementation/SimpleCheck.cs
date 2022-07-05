using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public class SimpleCheck : BaseCheck
    {
        private readonly Func<Task<CheckResult>> checkFunc;

        public SimpleCheck(Func<Task<CheckResult>> check, string name, string description, HashSet<string> tags,
            ImportanceType importance = ImportanceType.Advised, Priority priority = Priority.Medium) : base(name,
            description, tags, importance, priority)
        {
            checkFunc = check;
        }

        protected override async Task<CheckResult> GenerateCheckResult()
        {
            return await checkFunc();
        }
    }
}