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

            await validationProfile.Check();
        }

        /// <summary>
        /// Populates the ValidationProfile with all registered checks in <see cref="AvailableChecks"/> with specific tags.
        /// </summary>
        /// <param name="validationProfile"></param>
        /// <param name="checksTags">Checks tag filter</param>
        public static void PopulateChecks(this IValidationProfile validationProfile, params string[] checksTags)
        {
            foreach (var tag in checksTags)
            {
                foreach (var check in AvailableChecks.GetAllChecksWithTag(tag))
                {
                    validationProfile.CheckList.Add(check);
                }
            }
        }
        
        /// <summary>
        /// Removes from the ValidationProfile all registered checks in <see cref="AvailableChecks"/> with specific tags.
        /// </summary>
        /// <param name="validationProfile"></param>
        /// <param name="checksTags">Checks tag filter</param>
        public static void RemoveChecks(this IValidationProfile validationProfile, params string[] checksTags)
        {
            foreach (var tag in checksTags)
            {
                validationProfile.CheckList.RemoveWhere(c => c.Tags.Contains(tag));
            }
        }

        /// <summary>
        /// Register to this event to get a callback whenever an issue is fixed in the ValidationProfile.
        /// </summary>
        /// <param name="validationProfile"></param>
        /// <param name="func">Callback function</param>
        public static void OnAnyIssueFixed(this IValidationProfile validationProfile, System.Action<IIssue> func)
        {
            foreach (var check in validationProfile.CheckList)
            {
                check.OnIssueFixed -= func;
                check.OnIssueFixed += func;
            }
        }
    }
}