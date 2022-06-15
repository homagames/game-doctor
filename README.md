Game Doctor
==================================

An all in one tool to scan a Unity project and fix things. Ranging from performance optimisations to missing analytics.

## Installation

### Cloning
Clone or download this repository to your **Packages** folder.

### Using a scoped registry
Add a Scoped Registry in Edit → Project Settings → Package Manager.

>**Name:** TODO
>
>**Url:** TODO
>
>**Scope:** TODO



![](Documentation~/install.png)

### Basic usage of Game Doctor
Go to **Window > Homa Games > Game Doctor > Open Default Profile**.
##### Game Doctor main window : 
![](Documentation~/main-window.png)

### Validation Profiles
Game Doctor groups checks using Validation Profiles, they are a way to regroup checks and executing them together.

The Default Validation Profile contains all checks registered in the project. 

Game Doctor also comes with a [Tag based Validation Profile](Editor/Core/Implementation/TagBasedValidationProfile.cs).

Those are looking for all checks in the projects having specific tags and they can easily be created in the Game Doctor Settings window :
![](Documentation~/settings.png)


### Programmatic usage
You can create your own checks, issues and validation profile, by implementing [ICheck](Editor/Core/Model/ICheck.cs), [IIssue](Editor/Core/Model/IIssue.cs) and [IValidationProfile](Editor/Core/Model/IValidationProfile.cs).

If you want your checks or validation profiles to be available in the main Game Doctor window, use the [AvailableChecks](Editor/Core/Utilities/AvailableChecks.cs) and [AvailableProfiles](Editor/Core/Utilities/AvailableProfiles.cs) API :
```csharp
// The simplest place to call the register functions are on assembly reload using [InitializeOnLoadMethod]
AvailableChecks.RegisterCheck(myCheck);
AvailableProfiles.RegisterValidationProfile(myValidationProfile);
```

Here are examples of how to create a simple checks :
##### Creating a Check : 
```csharp
using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.GameDoctor.Core;
using UnityEditor;

// This checks if the Android Target SDK is set to Auto.
public class AndroidSettingsCheck : BaseCheck
{
    // Make sure to register the checks you created to the AvailableChecks API
    [InitializeOnLoadMethod]
    public static void RegisterCheck()
    {
        AvailableChecks.RegisterCheck(new AndroidSettingsCheck("settings"));
    }

    public AndroidSettingsCheck(params string[] tags) : base(
        "Android API Check",
        "Making sure Android Settings are right", new HashSet<string>(tags))
    {
    }

    // Generate a check result that contains potential issues
    protected override Task<CheckResult> GenerateCheckResult()
    {
        var result = new CheckResult();
        if (PlayerSettings.Android.targetSdkVersion != AndroidSdkVersions.AndroidApiLevelAuto)
            result.Issues.Add(
                // The issue can contain a fix for itself
                new SimpleIssue(() =>
                    {
                        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
                        return Task.CompletedTask;
                    },
                    "Wrong Android Target SDK", "Android Target SDK Version not set to Automatic."
                    , AutomationType.Automatic)
            );
        return Task.FromResult(result);
    }
}
```

##### Same check with interactive issue :
```csharp

```
## How to contribute


## License
[MIT](LICENCE.md)
