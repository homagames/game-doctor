using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public class StepBasedIssue : BaseIssue
    {
        protected List<Step> stepsList;
        protected bool _withInteractiveWindow = true;
        public Vector2 InteractiveWindowSize = default;

        public int CurrentStepIndex => stepsList?.FindIndex(step => !step.Done) ?? 0;

        public int StepCount => stepsList?.Count ?? 0;

        public Step CurrentStep => stepsList?.Find(step => !step.Done);

        protected StepBasedIssue()
        {
        }

        public StepBasedIssue(List<Step> steps, string name, string description,
            bool withInteractiveWindow = true, Priority priority = default) : base(name, description,
            AutomationType.Interactive, priority)
        {
            _withInteractiveWindow = withInteractiveWindow;
            stepsList = steps;
        }

        protected override async Task<bool> InternalFix()
        {
            if (_withInteractiveWindow)
                InteractiveStepWindow.Begin(this,InteractiveWindowSize);

            foreach (var step in stepsList)
            {
                step.Done = false;
            }

            foreach (var step in stepsList)
            {
                while (!step.Done)
                {
                    await Task.Delay(200);
                    if (!InteractiveStepWindow.IsOpen && _withInteractiveWindow)
                        return false;
                }
            }

            if (_withInteractiveWindow)
                InteractiveStepWindow.End();

            return true;
        }
    }
}