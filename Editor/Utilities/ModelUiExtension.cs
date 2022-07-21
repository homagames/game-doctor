using System.Linq;
using HomaGames.GameDoctor.Core;

namespace HomaGames.GameDoctor.Ui
{
    public struct PriorityCount
    {
        public int Low, Medium, High;

        public static PriorityCount operator+(PriorityCount p1, PriorityCount p2)
        {
            return new PriorityCount
            {
                Low = p1.Low + p2.Low,
                Medium = p1.Medium + p2.Medium,
                High = p1.High + p2.High,
            };
        }
    }
    
    public struct AutomationCount
    {
        public int Automatic, Interactive;

        public static AutomationCount operator+(AutomationCount p1, AutomationCount p2)
        {
            return new AutomationCount
            {
                Automatic = p1.Automatic + p2.Automatic,
                Interactive = p1.Interactive + p2.Interactive
            };
        }
    }

    public static class ModelUiExtension
    {
        public static PriorityCount GetPriorityCount(this IValidationProfile profile)
        {
            return profile.CheckList.Select(GetPriorityCount).Aggregate(new PriorityCount(), (count1, count2) => count1 + count2);
        }
    
        public static PriorityCount GetPriorityCount(this ICheck check)
        {
            PriorityCount output = new PriorityCount();
        
            if (check.CheckResult != null)
            {
                foreach (var issue in check.CheckResult.Issues)
                {
                    if (issue.HasBeenDismissed())
                        continue;
                    
                    switch (issue.Priority)
                    {
                        default:
                        case Priority.Low:
                            output.Low += 1;
                            break;
                        case Priority.Medium:
                            output.Medium += 1;
                            break;
                        case Priority.High:
                            output.High += 1;
                            break;
                    }
                }
            }

            return output;
        }

        public static AutomationCount GetAutomationCount(this IValidationProfile profile)
        {
            return profile.CheckList.Select(GetAutomationCount).Aggregate(new AutomationCount(), (count1, count2) => count1 + count2);
        }

        public static AutomationCount GetAutomationCount(this ICheck check)
        {
            AutomationCount output = new AutomationCount();
        
            if (check.CheckResult != null)
            {
                foreach (var issue in check.CheckResult.Issues)
                {
                    switch (issue.AutomationType)
                    {
                        default:
                        case AutomationType.Automatic:
                            output.Automatic += 1;
                            break;
                        case AutomationType.Interactive:
                            output.Interactive += 1;
                            break;
                    }
                }
            }

            return output;
        }
    }
}