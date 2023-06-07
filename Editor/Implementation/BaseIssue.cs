using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public abstract class BaseIssue : IIssue
    {
        protected BaseIssue()
        {
        }

        protected BaseIssue(string name, string description, AutomationType automationType = default,
            Priority priority = default)
        {
            Name = name;
            AutomationType = automationType;
            Priority = priority;
            Description = description;
        }

        public event Action<IIssue, bool> OnFixExecuted;
        public string Name { get; protected set; }
        public string Description { get; protected set; }

        public virtual void Draw()
        {
        }

        public AutomationType AutomationType { get; protected set; }
        public Priority Priority { get; protected set; }

        public async Task<bool> Fix()
        {
            var result = await InternalFix();
            OnFixExecuted?.Invoke(this, result);
            return result;
        }

        protected abstract Task<bool> InternalFix();
    }
}