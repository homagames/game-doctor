using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public class SimpleIssue : BaseIssue
    {
        private readonly System.Func<Task<bool>> fixFunc;

        public SimpleIssue(System.Func<Task<bool>> fix, string name,
            string description, AutomationType automationType = default,
            Priority priority = default) : base(name, description, automationType, priority)
        {
            fixFunc = fix;
        }

        protected override async Task<bool> InternalFix()
        {
            return await fixFunc();
        }
    }
}