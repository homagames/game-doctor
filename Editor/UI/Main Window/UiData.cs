using System;
using System.Collections.Generic;
using System.Linq;
using HomaGames.GameDoctor.Core;
using JetBrains.Annotations;
using UnityEditor.AnimatedValues;

namespace HomaGames.GameDoctor.Ui
{
    public partial class GameDoctorWindow
    {
        private abstract class BaseUiData
        {
            [NotNull]
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

            protected BaseUiData([NotNull] GameDoctorWindow window)
            {
                Window = window;
            }
        }

        private abstract class BaseFoldoutUiData : BaseUiData
        {
            [NotNull]
            public readonly AnimBool Expanded = new AnimBool(true);

            protected BaseFoldoutUiData([NotNull] GameDoctorWindow window) : base(window)
            {
                Expanded.valueChanged.AddListener(window.Repaint);
            }
        }

        private class ProfileUiData : BaseFoldoutUiData
        {
            public ProfileUiData([NotNull] GameDoctorWindow window) : base(window)
            {
            }
        }

        private class CheckUiData : BaseFoldoutUiData
        {
            public CheckUiData([NotNull] GameDoctorWindow window) : base(window)
            {
            }
        }

        private class IssueUiData : BaseUiData
        {
            public bool Fixed;

            public IssueUiData([NotNull] GameDoctorWindow window) : base(window)
            {
            }
        }

        [NotNull]
        private readonly Dictionary<IValidationProfile, ProfileUiData> ProfileUiDataBank =
            new Dictionary<IValidationProfile, ProfileUiData>();
        [NotNull]
        private readonly Dictionary<ICheck, CheckUiData> CheckUiDataBank = 
            new Dictionary<ICheck, CheckUiData>();
        [NotNull]
        private readonly Dictionary<IIssue, IssueUiData> IssueUiDataBank = 
            new Dictionary<IIssue, IssueUiData>(new IssueEqualityComparer());

        [NotNull]
        [MustUseReturnValue]
        private TValue TryGetOrCreate<TKey, TValue>([NotNull] IDictionary<TKey, TValue> dictionary, [NotNull] TKey key, [NotNull, InstantHandle] Func<TValue> creator)
        {
            if (dictionary.TryGetValue(key, out var output))
                return output;

            output = creator.Invoke();
            dictionary[key] = output;
            return output;
        }
        
        [NotNull]
        private ProfileUiData GetUiData([NotNull] IValidationProfile profile)
            => TryGetOrCreate(ProfileUiDataBank, profile, () => new ProfileUiData(this));

        [NotNull]
        private CheckUiData GetUiData([NotNull] ICheck check)
            => TryGetOrCreate(CheckUiDataBank, check, () => new CheckUiData(this));
        
        [NotNull]
        private IssueUiData GetUiData([NotNull] IIssue issue)
            => TryGetOrCreate(IssueUiDataBank, issue, () => new IssueUiData(this));

        [NotNull]
        [Pure]
        [LinqTunnel]
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