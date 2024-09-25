using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class Builder
{
    private static void BuildEmbeddedLinux(EmbeddedLinuxArchitecture architecture)
    {
        // Set architecture in BuildSettings
        EditorUserBuildSettings.selectedEmbeddedLinuxArchitecture = architecture;

        // Setup build options (e.g. scenes, build output location)
        var options = new BuildPlayerOptions
        {
            // Change to scenes from your project
            scenes = new[]
            {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/Soldier.unity",
                "Assets/Scenes/Sniper.unity",
            },
            // Change to location the output should go
            locationPathName = "../EmbeddedLinuxPlayer/",
            options = BuildOptions.CleanBuildCache | BuildOptions.StrictMode,
            target = BuildTarget.EmbeddedLinux
        };

        var report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build successful - Build written to {options.locationPathName}");
        }
        else if (report.summary.result == BuildResult.Failed)
        {
            Debug.LogError($"Build failed");
        }
    }

    public static void MyBuild()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/Soldier.unity",
                "Assets/Scenes/Sniper.unity",
            };
        buildPlayerOptions.locationPathName = "../BuildOutput/RelentlessAttack.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
        }
    }

    // This function will be called from the build process
    public static void build()
    {
        // Build EmbeddedLinux ARM64 Unity player
        MyBuild();
    }
}