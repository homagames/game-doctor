namespace HomaGames.GameDoctor.Core
{
    public static class CheckExtension
    {
        public static void FixAllIssues(this ICheck check)
        {
            if (check.CheckResult == null) return;
            foreach (var issue in check.CheckResult.Issues)
            {
                issue.Fix();
            }
        }
    }
}