using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Checks.Common;
using HomaGames.GameDoctor.Core;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace HomaGames.GameDoctor.Checks
{
    public class PlayerSettingsCheck : BaseCheck
    {
        public PlayerSettingsCheck() : base(
            "Player Settings Check",
            "Checking for issues in the Player Settings.", new HashSet<string>() {"settings"},
            ImportanceType.Mandatory,
            Priority.Medium)
        {
        }

        protected override async Task<CheckResult> GenerateCheckResult()
        {
            CheckResult result = new CheckResult();
            var list = Client.List(true, true);
            while (!list.IsCompleted)
                await Task.Delay(200);
            bool CompanyCheck() => PlayerSettings.companyName != "DefaultCompany";
            if (!CompanyCheck())
            {
                result.Issues.Add(new StepBasedIssue(new List<Step>()
                {
                    new OpenPlayerSettingsStep(),
                    new Step(CompanyCheck, "Rename your company to something else.", "")
                }, "Company Name", "You should name your company in Player Settings.", true, Priority.High));
            }

            bool BundleNameCheck() => PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android) ==
                                      PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);

            if (!BundleNameCheck())
            {
                result.Issues.Add(new StepBasedIssue(new List<Step>()
                    {
                        new OpenPlayerSettingsStep(),
                        new Step(BundleNameCheck, "Change your identifiers."
                            , "You android Package Name should be the same as your iOS Bundle Identifier.")
                    }, "Change your identifiers",
                    "Make sure you have the same identifiers for Android and iOS in the Player settings.", true,
                    Priority.High));
            }

            bool SplashImageCheck() => !Application.HasProLicense() || !PlayerSettings.SplashScreen.showUnityLogo;

            if (!SplashImageCheck())
            {
                result.Issues.Add(new SimpleIssue(() =>
                    {
                        PlayerSettings.SplashScreen.showUnityLogo = false;
                        return Task.FromResult(true);
                    }, "Unity Logo",
                    "You shouldn't show the Unity logo in the splashscreen if you have a pro licence."));
            }
            
            if(IconIssue.HasIssue)
                result.Issues.Add(new IconIssue());

            return result;
        }
    }
}