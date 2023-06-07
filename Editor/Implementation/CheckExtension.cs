using System.Threading.Tasks;
using JetBrains.Annotations;

namespace HomaGames.GameDoctor.Core
{
    public static class CheckExtension
    {
        /// <summary>
        /// The only way to compare two checks together. 
        /// </summary>
        /// <returns>True if both checks are the same</returns>
        public static bool Same(this ICheck check, ICheck otherCheck)
        {
            return check.Name == otherCheck.Name && check.Description == otherCheck.Description &&
                   check.GetType() == otherCheck.GetType();
        }

        /// <summary>
        /// Execute all fixes for generated issues inside a Check.
        /// </summary>
        /// <param name="check"></param>
        /// <param name="automationType">Filter by Automation Type</param>
        public static async Task FixAllIssues(this ICheck check,
            AutomationType automationType = AutomationType.Automatic)
        {
            if (check.CheckResult == null) return;
            foreach (var issue in check.CheckResult.Issues)
            {
                if (issue.AutomationType == automationType)
                    await issue.Fix();
            }
        }
        
        public static int GetHash([NotNull] this ICheck check)
        {
            unchecked
            {
                var hashCode = (check.Name != null ? check.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (check.Description != null ? check.Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) check.Importance;
                hashCode = (hashCode * 397) ^ (int) check.Priority;
                return hashCode;
            }
        }
    }
}