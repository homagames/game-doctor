using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace HomaGames.GameDoctor.Tests
{
    public class CoreTests
    {
        [SetUp]
        public void Setup()
        {
            EditorPrefs.SetBool("dummy_issue_fixed", false);
        }

        [UnityTest]
        public IEnumerator AllTestCustomImplementation()
        {
            var profile = new TagBasedValidationProfile("Homa Profile", "The default Homa Validation Profile");
            AvailableChecks.RegisterCheck(new DummyCheck());
            profile.Tags.Add("performance");
            yield return AllTestPass(profile);
        }

        [UnityTest]
        public IEnumerator AllTestInlineImplementation()
        {
            var profile = new TagBasedValidationProfile("Homa Profile", "The default Homa Validation Profile");
            var check = new SimpleCheck(async () =>
            {
                var ci = new CheckResult();
                await Task.Delay(100);
                if (!EditorPrefs.GetBool("dummy_issue_fixed"))
                {
                    ci.Issues.Add(new SimpleIssue(async () =>
                    {
                        await Task.Delay(1000);
                        EditorPrefs.SetBool("dummy_issue_fixed", true);
                        return true;
                    }, "Dummy Issue", ""));
                }

                return ci;
            }, "Dummy Check", "", new HashSet<string>() {"inline"});
            AvailableChecks.RegisterCheck(check);
            profile.Tags.Add("inline");
            yield return AllTestPass(profile);
        }

        [UnityTest]
        public IEnumerator FailingIssueFix()
        {
            var profile = new TagBasedValidationProfile("Homa Profile", "The default Homa Validation Profile");
            var check = new SimpleCheck(async () =>
            {
                var ci = new CheckResult();
                await Task.Delay(100);
                if (!EditorPrefs.GetBool("dummy_issue_fixed"))
                {
                    ci.Issues.Add(new SimpleIssue(async () =>
                    {
                        await Task.Delay(1000);
                        //EditorPrefs.SetBool("dummy_issue_fixed", true);
                        return false;
                    }, "Dummy Issue", ""));
                }

                return ci;
            }, "Dummy Check", "", new HashSet<string>() {"failing"});
            AvailableChecks.RegisterCheck(check);
            profile.Tags.Add("failing");
            yield return AllTestPass(profile, false);
        }

        private IEnumerator AllTestPass(IValidationProfile profile, bool fixWorks = true)
        {
            List<bool> fixedIssuesState = new List<bool>();

            profile.OnAnyIssueFixed((c, i, actuallyFixed) => fixedIssuesState.Add(actuallyFixed));

            yield return TestUtils.AsIEnumerator(profile.Check());
            Assert.True(profile.CheckList.Count == 1);
            Assert.True(profile.CheckList.First().CheckResult.Issues.Count == 1);
            yield return TestUtils.AsIEnumerator(profile.Fix());
            yield return TestUtils.AsIEnumerator(profile.Check());
            Assert.True(profile.CheckList.Count == 1);
            Assert.True(profile.CheckList.First().CheckResult.Issues.Count == (fixWorks ? 0 : 1));

            Assert.True(fixWorks == fixedIssuesState[0]);

            yield return TestUtils.AsIEnumerator(profile.Fix());

            yield return TestUtils.AsIEnumerator(profile.Check());
            Assert.True(profile.CheckList.Count == 1);
            if (fixWorks)
                Assert.True(profile.CheckList.First().CheckResult.Issues.Count == 0);
        }

        [Test]
        public void DefaultValidationProfile()
        {
            Assert.True(AvailableProfiles.GetDefaultValidationProfile() != null);
        }
    }

    public class DummyIssue : BaseIssue
    {
        public DummyIssue(AutomationType automationType, Priority priority) : base("Dummy Issue", "",
            automationType, priority)
        {
        }

        protected override async Task<bool> InternalFix()
        {
            await Task.Delay(2000);
            EditorPrefs.SetBool("dummy_issue_fixed", true);
            return true;
        }
    }

    public class DummyCheck : BaseCheck
    {
        protected override async Task<CheckResult> GenerateCheckResult()
        {
            var ci = new CheckResult();
            await Task.Delay(100);
            if (!EditorPrefs.GetBool("dummy_issue_fixed"))
            {
                ci.Issues.Add(new DummyIssue(AutomationType.Automatic, Priority.Low));
            }

            return ci;
        }

        public DummyCheck(ImportanceType importance = ImportanceType.Advised, Priority priority = Priority.Medium) :
            base("Dummy Check",
                "Test check", new HashSet<string>() {"performance"}, importance, priority)
        {
        }
    }
}