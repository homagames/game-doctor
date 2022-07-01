using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor.PackageManager;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class PackageManagerCheck : BaseCheck
    {
        public PackageManagerCheck() : base(
            "Package Manager Check",
            "Checking for issues in the Package Manager settings.", new HashSet<string>() {"settings"},
            ImportanceType.Mandatory,
            Priority.Medium)
        {
        }

        protected override async Task<CheckResult> GenerateCheckResult()
        {
            CheckResult result = new CheckResult();
            var list = Client.List(true, true);
            while (!list.IsCompleted)
                await Task.Delay(200);
            return result;
        }
    }
}