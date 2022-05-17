using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;

namespace HomaGames.GameDoctor.Core
{
    public abstract class BaseIssue : IIssue
    {
        protected BaseIssue(string name, string description, AutomationType automationType = default,
            Priority priority = default)
        {
            Name = name;
            AutomationType = automationType;
            Priority = priority;
            Description = description;
        }

        public event Action<IIssue> OnFixExecuted;
        public string Name { get; }
        public string Description { get; }

        public virtual void Draw()
        {
            EditorGUILayout.TextArea(Description);
        }

        public ICheck Check { get; }
        public AutomationType AutomationType { get; }
        public Priority Priority { get; }

        public async Task Fix()
        {
            await InternalFix();
            OnFixExecuted?.Invoke(this);
        }

        protected abstract Task InternalFix();
    }
}