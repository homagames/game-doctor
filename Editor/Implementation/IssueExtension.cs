using JetBrains.Annotations;

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
        
        public static int GetHash([NotNull] this IIssue issue)
        {
            unchecked
            {
                var hashCode = (issue.Name != null ? issue.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (issue.Description != null ? issue.Description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int) issue.AutomationType;
                hashCode = (hashCode * 397) ^ (int) issue.Priority;
                return hashCode;
            }
        }
    }
}