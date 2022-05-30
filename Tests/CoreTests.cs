using System.Collections;
using System.Collections.Generic;
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
            var profile = new BaseValidationProfile("Homa Profile", "The default Homa Validation Profile");
            AvailableChecks.RegisterCheck(new DummyCheck());
            profile.PopulateChecks("performance");
            yield return AllTestPass(profile);
        }

        [UnityTest]
        public IEnumerator AllTestInlineImplementation()
        {
            var profile = new BaseValidationProfile("Homa Profile", "The default Homa Validation Profile");
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
                    }, "Dummy Issue", ""));
                }

                return ci;
            }, "Dummy Check", "", new List<string>() {"inline"});
            AvailableChecks.RegisterCheck(check);
            profile.PopulateChecks("inline");
            yield return AllTestPass(profile);
        }

        [UnityTest]
        public IEnumerator FailingIssueFix()
        {
            var profile = new BaseValidationProfile("Homa Profile", "The default Homa Validation Profile");
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
                    }, "Dummy Issue", ""));
                }

                return ci;
            }, "Dummy Check", "", new List<string>() {"failing"});
            AvailableChecks.RegisterCheck(check);
            profile.PopulateChecks("failing");
            yield return AllTestPass(profile, false);
        }

        private IEnumerator AllTestPass(IValidationProfile profile, bool fixWorks = true)
        {
            List<IIssue> fixedIssues = new List<IIssue>();
            profile.OnAnyIssueFixed(i => fixedIssues.Add(i));

            yield return TestUtils.AsIEnumerator(profile.Check());
            Assert.True(profile.CheckList.Count == 1);
            Assert.True(profile.CheckList[0].CheckResult.Issues.Count == 1);
            yield return TestUtils.AsIEnumerator(profile.Fix());
            Assert.True(profile.CheckList.Count == 1);
            Assert.True(profile.CheckList[0].CheckResult.Issues.Count == (fixWorks ? 0 : 1));

            if (fixWorks)
                Assert.True(fixedIssues[0].Name == "Dummy Issue");
            else
                Assert.True(fixedIssues.Count == 0);

            fixedIssues.Clear();
            yield return TestUtils.AsIEnumerator(profile.Fix());
            Assert.True(fixedIssues.Count == 0);

            yield return TestUtils.AsIEnumerator(profile.Check());
            Assert.True(profile.CheckList.Count == 1);
            if(fixWorks)
                Assert.True(profile.CheckList[0].CheckResult.Issues.Count == 0);
        }
    }

    public class DummyIssue : BaseIssue
    {
        public DummyIssue(AutomationType automationType, Priority priority) : base("Dummy Issue", "",
            automationType, priority)
        {
        }

        protected override async Task InternalFix()
        {
            await Task.Delay(2000);
            EditorPrefs.SetBool("dummy_issue_fixed", true);
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
                "Test check", new List<string>() {"performance"}, importance, priority)
        {
        }
    }
}