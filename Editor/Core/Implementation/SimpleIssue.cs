using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public class SimpleIssue : BaseIssue
    {
        private readonly System.Func<Task> fixFunc;

        public SimpleIssue(System.Func<Task> fix, string name,
            string description, AutomationType automationType = default,
            Priority priority = default) : base(name, description, automationType, priority)
        {
            fixFunc = fix;
        }

        protected override async Task InternalFix()
        {
            await fixFunc();
        }
    }
}