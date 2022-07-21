using JetBrains.Annotations;

namespace HomaGames.GameDoctor.Core
{
    public static class IssueHash
    {
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