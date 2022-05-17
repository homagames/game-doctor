using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public static class ValidationProfileExtension
    {
        public static async Task Fix(this IValidationProfile validationProfile)
        {
            foreach (var check in validationProfile.CheckList)
            foreach (var issue in check.CheckResult.Issues)
            {
                await issue.Fix();
            }

            await validationProfile.Check();
        }

        public static void Populate(this IValidationProfile validationProfile, params string[] checksTags)
        {
            foreach (var tag in checksTags)
            {
                validationProfile.CheckList.AddRange(AvailableChecks.GetAllChecksWithTag(tag));
            }
        }

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