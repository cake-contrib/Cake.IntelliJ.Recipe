// #l ./Cake.Recipe/packages.cake_ex


IntelliJBuildParameters.Tasks.CreatePluginPackagesTask = Task("Create-Plugin-Packages")
    .IsDependentOn("IntelliJBuild")
    .Does<BuildVersion>((context, buildVersion) =>
{
    Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithLogLevel(IntelliJBuildParameters.GradleVerbosity)
        .WithTask("buildPlugin")
        .WithProjectProperty("pluginVersion", buildVersion.SemVersion)
        .Run(); 

    // copy zip to output
    var outputFolder = IntelliJBuildParameters.Paths.Directories.PluginPackages;
    EnsureDirectoryExists(outputFolder);
    var files = GetFiles(IntelliJBuildParameters.PluginPackOutputPath + "/**/*");
    if (files.Any())
    {
        CopyFiles(files, outputFolder, true);
    }
    else
    {
        Warning("No files were found in the pack output directory: '{0}'", IntelliJBuildParameters.PluginPackOutputPath);
    }
});

BuildParameters.Tasks.PublishPreReleasePackagesTask = Task("Publish-PreRelease-Packages")
    .WithCriteria(() => !BuildParameters.IsLocalBuild || BuildParameters.ForceContinuousIntegration, "Skipping because this is a local build, and force isn't being applied")
    .WithCriteria(() => !BuildParameters.IsTagged, "Skipping because current commit is tagged")
    .WithCriteria(() => BuildParameters.PreferredBuildAgentOperatingSystem == BuildParameters.BuildAgentOperatingSystem, "Not running on preferred build agent operating system")
    .WithCriteria(() => BuildParameters.PreferredBuildProviderType == BuildParameters.BuildProvider.Type, "Not running on preferred build provider type")
    .IsDependentOn("IntelliJPackage")
    .Does<BuildVersion>((context, buildVersion) => 
{
    PushPluginToMarketplace(Context, buildVersion, false);
})
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-PreRelease-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});

BuildParameters.Tasks.PublishReleasePackagesTask = Task("Publish-Release-Packages")
    .WithCriteria(() => !BuildParameters.IsLocalBuild || BuildParameters.ForceContinuousIntegration, "Skipping because this is a local build, and force isn't being applied")
    .WithCriteria(() => BuildParameters.IsTagged, "Skipping because current commit is not tagged")
    .WithCriteria(() => BuildParameters.PreferredBuildAgentOperatingSystem == BuildParameters.BuildAgentOperatingSystem, "Not running on preferred build agent operating system")
    .WithCriteria(() => BuildParameters.PreferredBuildProviderType == BuildParameters.BuildProvider.Type, "Not running on preferred build provider type")
    .IsDependentOn("IntelliJPackage")
    .Does<BuildVersion>((context, buildVersion) =>
{
    PushPluginToMarketplace(Context, buildVersion, true);

    BuildParameters.PublishReleasePackagesWasSuccessful = true;
})
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-Release-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});

IntelliJBuildParameters.Tasks.ForcePublishPlugin = Task("Force-Publish-Plugin")
    .IsDependentOn("IntelliJPackage")
    .Does<BuildVersion>((context, buildVersion) =>
{
    PushPluginToMarketplace(Context, buildVersion, true);
});

public void PushPluginToMarketplace(ICakeContext context, BuildVersion buildVersion, bool isTaggedRelease) 
{
    var channel = IntelliJBuildParameters.PluginCiBuildChannel;

    if(!isTaggedRelease && !IntelliJBuildParameters.ShouldPublishPluginCiBuilds)
    {
        context.Information("Publish of CI-builds to JetBrains Marketplace is disabled.");
        return;
    }

    if(isTaggedRelease) 
    {
        if(BuildParameters.BranchType == BranchType.Master) 
        {
            channel = IntelliJBuildParameters.PluginReleaseChannel;
        } 
        else 
        {
            channel = IntelliJBuildParameters.PluginPreReleaseChannel;
        }
    }

    context.Information("Publishing to channel: {0}", channel);

    // TODO: This uses the publish configuration from build.gradle.kts - should we somehow supply our own configuration?
    context.Gradle()
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithLogLevel(IntelliJBuildParameters.GradleVerbosity)
        .WithProjectProperty("pluginVersion", buildVersion.SemVersion)
        .WithProjectProperty(IntelliJBuildParameters.PluginChannelGradleProperty, channel)
        .WithTask("publishPlugin")
        .Run(); 

}
