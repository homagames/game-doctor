using System.Collections.Generic;
using HomaGames.GameDoctor.Core;
using JetBrains.Annotations;

public class IssueEqualityComparer : IEqualityComparer<IIssue>
{ 
    [Pure]
    public bool Equals(IIssue x, IIssue y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.Name == y.Name && x.Description == y.Description && x.AutomationType == y.AutomationType && x.Priority == y.Priority;
    }

    [Pure]
    public int GetHashCode(IIssue obj)
    {
        return obj.GetHash();
    }
}
