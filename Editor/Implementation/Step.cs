using System;
using UnityEditor;
using UnityEngine;

namespace HomaGames.GameDoctor.Core
{
    public class Step
    {
        protected string _name;
        /// <summary>
        /// Name of the Step
        /// </summary>
        public string Name => _name;

        /// <summary>
        /// True if this Step is completed
        /// </summary>
        public bool Done;

        protected Func<bool> _predicateFunc;
        /// <summary>
        /// True if we can go to the next step 
        /// </summary>
        public bool Predicate => _predicateFunc?.Invoke() ?? false;

        /// <summary>
        /// Provides a way to draw custom UI inside the step description
        /// </summary>
        protected Action<StepBasedIssue, Step> _drawFunc;

        protected Step()
        {
            
        }
        
        public Step(Func<bool> predicate, string name, Action<StepBasedIssue, Step> draw = null)
        {
            _predicateFunc = predicate;
            _name = name;
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