using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

class BuildScript
{
	static readonly BuildOptions releaseBuildOptions = BuildOptions.None;
	static readonly BuildOptions debugBuildOptions = BuildOptions.Development | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging | BuildOptions.AutoRunPlayer;

	[MenuItem("Build/Release")]
	static void PerformBuildRelease()
	{
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
		buildPlayerOptions.target = BuildTarget.StandaloneWindows64;

		buildPlayerOptions.options = releaseBuildOptions;
		buildPlayerOptions.locationPathName = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "Release", Application.productName + ".exe");

		BuildPipeline.BuildPlayer(buildPlayerOptions);
	}

	[MenuItem("Build/Debug")]
	static void PerformBuildDebug()
	{
		BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
		buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
		buildPlayerOptions.target = BuildTarget.StandaloneWindows64;

		buildPlayerOptions.options = debugBuildOptions;
		buildPlayerOptions.locationPathName = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "Debug", Application.productName + "_debug.exe");

		BuildPipeline.BuildPlayer(buildPlayerOptions);
	}
}