using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;

namespace HomaGames.GameDoctor.Core
{
    public class StepBasedIssue : BaseIssue
    {
        public class Step
        {
            public readonly string Name;
            public bool Done;
            public readonly Func<Task> Action;
            public readonly Action<StepBasedIssue, Step> Draw;

            public Step(Func<Task> action, string name, Action<StepBasedIssue, Step> draw = null)
            {
                Action = action;
                Name = name;
                Draw = draw;
            }
        }

        private readonly List<Step> stepsList;

        public StepBasedIssue(List<Step> steps, string name, string description,
            AutomationType automationType = default,
            Priority priority = default) : base(name, description, automationType, priority)
        {
            stepsList = steps;
        }

        public override void Draw()
        {
            base.Draw();
            foreach (var step in stepsList)
            {
                if (step.Done)
                    EditorGUILayout.LabelField(step.Name, EditorStyles.boldLabel);
                else
                    EditorGUILayout.LabelField(step.Name);
                step.Draw?.Invoke(this, step);
            }
        }

        protected override async Task InternalFix()
        {
            foreach (var step in stepsList)
            {
                step.Done = false;
            }

            foreach (var step in stepsList)
            {
                await step.Action();
                step.Done = true;
            }
        }
    }
}