using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class Builder
{
    // This function will be called from the build process
    public static void Build()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] {
                "Assets/Scenes/MainMenu.unity",
                "Assets/Scenes/Soldier.unity",
                "Assets/Scenes/Sniper.unity",
            };
        buildPlayerOptions.locationPathName = "/home/connor/Documents/Results/BuildOutput/";
        buildPlayerOptions.target = BuildTarget.WebGL;
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
}