using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public static class ValidationProfileExtension
    {
        /// <summary>
        /// Fixes all issues inside the Validation Profile.
        /// </summary>
        public static async Task Fix(this IValidationProfile validationProfile)
        {
            foreach (var check in validationProfile.CheckList)
            foreach (var issue in check.CheckResult.Issues)
            {
                await issue.Fix();
            }
        }
        
        /// <summary>
        /// Populates CheckResults for all Checks in the CheckList.
        /// </summary>
        public static async Task Check(this IValidationProfile validationProfile)
        {
            foreach (var check in validationProfile.CheckList)
                await check.Execute();
        }

        /// <summary>
        /// Register to this event to get a callback whenever an issue is fixed in the ValidationProfile.
        /// </summary>
        /// <param name="validationProfile"></param>
        /// <param name="func">Callback function</param>
        public static void OnAnyIssueFixed(this IValidationProfile validationProfile, System.Action<ICheck,IIssue,bool> func)
        {
            foreach (var check in validationProfile.CheckList)
            {
                check.OnIssueFixExecuted -= func;
                check.OnIssueFixExecuted += func;
            }
        }
    }
}