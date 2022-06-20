using System;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public class Step
    {
        /// <summary>
        /// Name of the Step
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// True if this Step is completed
        /// </summary>
        public bool Done;

        /// <summary>
        /// True if we can go to the next step 
        /// </summary>
        public readonly Func<bool> Predicate;

        /// <summary>
        /// Provides a way to draw custom UI inside the step description
        /// </summary>
        private readonly Action<StepBasedIssue, Step> _drawFunc;

        public Step(Func<bool> predicate, string name, Action<StepBasedIssue, Step> draw = null)
        {
            Predicate = predicate;
            Name = name;
            _drawFunc = draw;
        }

        public Step(Func<bool> predicate, string name, string drawText) : this(predicate, name)
        {
            _drawFunc = (issue, step) => { GUILayout.Label(drawText, EditorStyles.wordWrappedLabel); };
        }

        public void Draw(StepBasedIssue stepBasedIssue)
        {
            _drawFunc?.Invoke(stepBasedIssue, this);
        }
    }
}