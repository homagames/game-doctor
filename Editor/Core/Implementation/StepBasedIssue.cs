using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;

namespace HomaGames.GameDoctor.Core
{
    public class StepBasedIssue : BaseIssue
    {
        private readonly List<Step> stepsList;
        private bool _withInteractiveWindow = true;

        public int CurrentStepIndex => stepsList?.FindIndex(step => !step.Done) ?? 0;

        public int StepCount => stepsList?.Count ?? 0;

        public Step CurrentStep => stepsList?.Find(step => !step.Done);

        public StepBasedIssue(List<Step> steps, string name, string description,
            bool withInteractiveWindow = true, Priority priority = default) : base(name, description,
            AutomationType.Interactive, priority)
        {
            _withInteractiveWindow = withInteractiveWindow;
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
                step.Draw(this);
            }
        }

        protected override async Task InternalFix()
        {
            var window = InteractiveStepWindow.Create(this);
            
            if (_withInteractiveWindow)
                window.ShowUtility();
            foreach (var step in stepsList)
            {
                step.Done = false;
            }

            foreach (var step in stepsList)
            {
                await step.Action();
                step.Done = true;
            }

            if (_withInteractiveWindow)
                window.Close();
        }
    }
}