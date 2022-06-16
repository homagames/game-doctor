using System;
using System.Threading.Tasks;

namespace HomaGames.GameDoctor.Core
{
    public class Step
    {
        public readonly string Name;
        public bool Done;
        public readonly Func<Task> Action;
        private readonly Action<StepBasedIssue, Step> _drawFunc;

        public Step(Func<Task> action, string name, Action<StepBasedIssue, Step> draw = null)
        {
            Action = action;
            Name = name;
            _drawFunc = draw;
        }

        public void Draw(StepBasedIssue stepBasedIssue)
        {
            _drawFunc?.Invoke(stepBasedIssue,this);
        }
    }
}