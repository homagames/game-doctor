using System.Threading.Tasks;

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
    }
}