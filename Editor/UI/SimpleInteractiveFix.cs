using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SimpleInteractiveFix
{
    public class Step
    {
        public string Description;
        public Action OnStepStart;
        public Func<bool> CanContinue = null;
    }

    public List<Step> Steps;

    public async Task<bool> Start(string windowTitle = "Issue fixing helper")
    {
        return await EditorWindow.GetWindow<Window>(true, windowTitle).StartInteractiveFix(Steps);
    }

    public class Window : EditorWindow
    {
        private List<Step> Steps;
        private int CurrentStepIndex;
        private int MaxIndexReached = -1;

        private bool FixCompleted;

        private AwaitableEvent<bool> OnFinished;

        private Step CurrentStep => Steps[CurrentStepIndex];
        
        public AwaitableEvent<bool> StartInteractiveFix(List<Step> steps)
        {
            Steps = steps;
            MaxIndexReached = -1;
            SetCurrentStepIndex(0);

            OnFinished = new AwaitableEvent<bool>();
            return OnFinished;
        }

        private void OnGUI()
        {
            if (Steps == null)
            {
                Close();
                return;
            }
            
            EditorGUILayout.LabelField($"Step {CurrentStepIndex+1}/{Steps.Count}", EditorStyles.boldLabel);
            
            EditorGUILayout.LabelField(CurrentStep.Description, new GUIStyle(GUI.skin.label) { wordWrap = true });

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(CurrentStepIndex == 0);
            if (GUILayout.Button("Previous Step"))
            {
                SetCurrentStepIndex(CurrentStepIndex-1);
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(!(CurrentStep.CanContinue?.Invoke() ?? true));
            if (CurrentStepIndex == Steps.Count - 1)
            {
                if (GUILayout.Button("Finish"))
                {
                    FixCompleted = true;
                    Close();
                }
            }
            else
            {
                if (GUILayout.Button("Next Step"))
                {
                    SetCurrentStepIndex(CurrentStepIndex+1);
                }
            }
            EditorGUI.EndDisabledGroup();
            
            EditorGUILayout.EndHorizontal();
        }

        private void OnDestroy()
        {
            OnFinished?.Invoke(FixCompleted);
        }

        private void SetCurrentStepIndex(int newIndex)
        {
            CurrentStepIndex = newIndex;
            if (MaxIndexReached < CurrentStepIndex)
            {
                MaxIndexReached = CurrentStepIndex;
                CurrentStep.OnStepStart?.Invoke();
            }
        }

        public class AwaitableEvent<T> : INotifyCompletion
        {
            public bool IsCompleted { get; private set; }
            public bool IsFailed { get; private set; }
            public Exception Exception { get; private set; }
            private Action OnCompletedAction;

            private T Value;
        
            public AwaitableEvent<T> GetAwaiter()
            {
                return this;
            }

            public static implicit operator Task<T>(AwaitableEvent<T> taskBase)
            {
                return taskBase.ToTask();
            }

            private async Task<T> ToTask()
            {
                return await this;
            }

            public void Invoke(T value)
            {
                IsCompleted = true;
                Value = value;
                
                OnCompletedAction?.Invoke();
            }
        
            public void OnTaskFailed(Exception exception)
            {
                IsFailed = true;
                Exception = exception;
                OnCompletedAction?.Invoke();
            }

            public T GetResult()
            {
                if (IsFailed)
                    throw Exception;

                return Value;
            }

            public void OnCompleted(Action continuation)
            {
                if (IsCompleted)
                    continuation.Invoke();
                else
                {
                    SynchronizationContext capturedSynchronizationContext = SynchronizationContext.Current;
                    if (capturedSynchronizationContext != null)
                    {
                        OnCompletedAction += () =>
                        {
                            capturedSynchronizationContext.Post(_ => continuation(), null);
                        };
                    }
                    else
                    {
                        OnCompletedAction += continuation;
                    }
                }
            }
        }
    }
}
