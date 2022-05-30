namespace HomaGames.GameDoctor.Core
{
    public static class IssueExtension
    {
        /// <summary>
        /// The only way to compare two issues together. 
        /// </summary>
        /// <returns>True if both issues are the same</returns>
        public static bool Same(this IIssue issue, IIssue otherIssue)
        {
            return issue.Name == otherIssue.Name && issue.Description == otherIssue.Description
                                                 && issue.GetType() == otherIssue.GetType();
        }
    }
}