using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;

namespace HomaGames.GameDoctor.Core
{
    public abstract class BaseIssue : IIssue
    {
        public BaseIssue(string name, ICheck check, AutomatableType automatableType, Priority priority,
            string description)
        {
            Name = name;
            Check = check;
            AutomatableType = automatableType;
            Priority = priority;
            Description = description;
        }

        public event Action<IIssue> OnExecuted;
        public string Name { get; }
        public virtual string Description { get; }

        public virtual void Draw()
        {
            EditorGUILayout.TextArea(Description);
        }

        public ICheck Check { get; }
        public AutomatableType AutomatableType { get; }
        public Priority Priority { get; }

        public async Task Fix()
        {
            await InternalFix();
            OnExecuted?.Invoke(this);
        }

        protected abstract Task InternalFix();

        public override int GetHashCode()
        {
            return (GetType().FullName + Name).GetHashCode();
        }
    }
}