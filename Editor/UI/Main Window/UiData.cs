using System;
using System.Collections.Generic;
using System.Linq;
using HomaGames.GameDoctor.Core;
using UnityEditor.AnimatedValues;

namespace HomaGames.GameDoctor.Ui
{
    public partial class GameDoctorWindow
    {
        private abstract class BaseUiData
        {
            protected readonly GameDoctorWindow Window;

            private bool _selected;

            public bool Selected
            {
                get => _selected;
                set
                {
                    if (value)
                    {
                        foreach (var otherData in Window.GetAllUiData().Where(data => data != this))
                        {
                            otherData.Selected = false;
                        }
                    }

                    _selected = value;
                }
            }

            protected BaseUiData(GameDoctorWindow window)
            {
                Window = window;
            }
        }

        private abstract class BaseFoldoutUiData : BaseUiData
        {
            public readonly AnimBool Expanded = new AnimBool(true);

            protected BaseFoldoutUiData(GameDoctorWindow window) : base(window)
            {
                Expanded.valueChanged.AddListener(window.Repaint);
            }
        }

        private class ProfileUiData : BaseFoldoutUiData
        {
            public ProfileUiData(GameDoctorWindow window) : base(window)
            {
            }
        }

        private class CheckUiData : BaseFoldoutUiData
        {
            public CheckUiData(GameDoctorWindow window) : base(window)
            {
            }
        }

        private class IssueUiData : BaseUiData
        {
            public bool Fixed;

            public IssueUiData(GameDoctorWindow window) : base(window)
            {
            }
        }

        private readonly Dictionary<IValidationProfile, ProfileUiData> ProfileUiDataBank =
            new Dictionary<IValidationProfile, ProfileUiData>();
        private readonly Dictionary<ICheck, CheckUiData> CheckUiDataBank = new Dictionary<ICheck, CheckUiData>();
        private readonly Dictionary<IIssue, IssueUiData> IssueUiDataBank = new Dictionary<IIssue, IssueUiData>(new IssueEqualityComparer());

        private TValue TryGetOrCreate<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> creator)
        {
            if (dictionary.TryGetValue(key, out var output))
                return output;

            output = creator.Invoke();
            dictionary[key] = output;
            return output;
        }

        private ProfileUiData GetUiData(IValidationProfile profile)
            => TryGetOrCreate(ProfileUiDataBank, profile, () => new ProfileUiData(this));

        private CheckUiData GetUiData(ICheck check)
            => TryGetOrCreate(CheckUiDataBank, check, () => new CheckUiData(this));

        private IssueUiData GetUiData(IIssue issue)
            => TryGetOrCreate(IssueUiDataBank, issue, () => new IssueUiData(this));

        private IEnumerable<BaseUiData> GetAllUiData()
        {
            return ProfileUiDataBank.Values.Union<BaseUiData>(CheckUiDataBank.Values).Union(IssueUiDataBank.Values);
        }
        
        private void OnIssueListChanged()
        {
            var issueList = GetAllIssues().ToList();

            foreach (var displayedIssue in IssueUiDataBank.Keys.ToList())
            {
                if (!issueList.Contains(displayedIssue, IssueUiDataBank.Comparer))
                {
                    IssueUiDataBank.Remove(displayedIssue);
                }
            }
        }
    }
}