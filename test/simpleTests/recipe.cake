#load ../Cake.IntelliJ.Recipe.cake                                                                                                                                                                                                                               Environment.SetVariableNames();

Environment.SetVariableNames();

IntelliJBuildParameters.SetParameters(
  context: Context,
  buildSystem: BuildSystem,
  sourceDirectoryPath: "./src",
  title: "Sample Plugin",
  repositoryName: "sample-plugin",
  repositoryOwner: "nils-a",
  marketplaceId: "12345-sample-plugin",
  shouldCalculateVersion: false,
  preferredBuildProviderType: BuildProviderType.GitHubActions,
  preferredBuildAgentOperatingSystem: PlatformFamily.Linux
);

IntelliJBuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context);

IntelliJBuild.Run();