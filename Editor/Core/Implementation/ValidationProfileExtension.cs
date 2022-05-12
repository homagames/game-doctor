using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public static class ValidationProfileExtension
    {
        public static async Task Fix(this IValidationProfile validationProfile)
        {
            foreach (var check in validationProfile.CheckList)
                for (int j = check.CheckResult.Issues.Count - 1; j >= 0; j--)
                {
                    var issue = check.CheckResult.Issues[j];
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
    }
}