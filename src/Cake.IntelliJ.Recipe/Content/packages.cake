BuildParameters.Tasks.CreateChocolateyPackagesTask = Task("Create-Chocolatey-Packages")
    .IsDependentOn("Clean")
    .WithCriteria(() => BuildParameters.ShouldRunChocolatey, "Skipping because execution of Chocolatey has been disabled")
    .WithCriteria(() => BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows, "Skipping because not running on Windows")
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory), "Skipping because Chocolatey nuspec directory is missing")
    .Does<BuildVersion>((context, buildVersion) =>
{
    var nuspecFiles = GetFiles(BuildParameters.Paths.Directories.ChocolateyNuspecDirectory + "/**/*.nuspec");

    EnsureDirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages);

    foreach (var nuspecFile in nuspecFiles)
    {
        // TODO: Add the release notes
        // ReleaseNotes = BuildParameters.ReleaseNotes.Notes.ToArray(),

        // Create package.
        ChocolateyPack(nuspecFile, new ChocolateyPackSettings {
            Version = buildVersion.SemVersion,
            OutputDirectory = BuildParameters.Paths.Directories.ChocolateyPackages,
            WorkingDirectory = BuildParameters.Paths.Directories.PublishedApplications
        });
    }
});

BuildParameters.Tasks.CreatePluginPackagesTask = Task("Create-Plugin-Packages")
    .IsDependentOn("Build")
    .Does<BuildVersion>((context, buildVersion) =>
{
    Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithTask("buildPlugin")
        .WithArguments($"-PpluginVersion=\"{buildVersion.SemVersion}\"") // workaround for cake.gradle implementing WithProperty("pluginVersion", "3.2.1")
        .Run(); 

    // copy zip to output
    var outputFolder = BuildParameters.Paths.Directories.PluginPackages;
    EnsureDirectoryExists(outputFolder);
    var files = GetFiles(BuildParameters.PluginPackOutputPath + "/**/*");
    if (files.Any())
    {
        CopyFiles(files, outputFolder, true);
    }
    else
    {
        Warning("No files were found in the pack output directory: '{0}'", BuildParameters.PluginPackOutputPath);
    }
});

BuildParameters.Tasks.PublishPreReleasePackagesTask = Task("Publish-PreRelease-Packages")
    .WithCriteria(() => !BuildParameters.IsLocalBuild || BuildParameters.ForceContinuousIntegration, "Skipping because this is a local build, and force isn't being applied")
    .WithCriteria(() => !BuildParameters.IsTagged, "Skipping because current commit is tagged")
    .WithCriteria(() => BuildParameters.PreferredBuildAgentOperatingSystem == BuildParameters.BuildAgentOperatingSystem, "Not running on preferred build agent operating system")
    .WithCriteria(() => BuildParameters.PreferredBuildProviderType == BuildParameters.BuildProvider.Type, "Not running on preferred build provider type")
    .IsDependentOn("Package")
    .Does<BuildVersion>((context, buildVersion) => 
{
    var chocolateySources = BuildParameters.PackageSources.Where(p => p.Type == FeedType.Chocolatey && p.IsRelease == false).ToList();

    PushChocolateyPackages(Context, false, chocolateySources);

    if(BuildParameters.ShouldPublishPreReleasePlugin) 
    {
        // TODO: This uses the publish configuration from build.gradle.kts - we should somehow supply our own configuration
        Gradle
            .FromPath(BuildParameters.SourceDirectoryPath)
            .WithTask("publishPlugin")
            .WithArguments($"-PpluginVersion=\"{buildVersion.SemVersion}\"") // workaround for cake.gradle implementing WithProperty("pluginVersion", "3.2.1")
            .Run(); 
    }
    else 
    {
        Warning("publish of PreRelease plugin is disabled.");
    }
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
    .IsDependentOn("Package")
    .Does<BuildVersion>((context, buildVersion) =>
{
    var chocolateySources = BuildParameters.PackageSources.Where(p => p.Type == FeedType.Chocolatey && p.IsRelease == true).ToList();

    PushChocolateyPackages(Context, true, chocolateySources);

    // TODO: This uses the publish configuration from build.gradle.kts - we should somehow supply our own configuration
     Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithTask("publishPlugin")
        .WithArguments($"-PpluginVersion=\"{buildVersion.SemVersion}\"") // workaround for cake.gradle implementing WithProperty("pluginVersion", "3.2.1")
        .Run(); 

    BuildParameters.PublishReleasePackagesWasSuccessful = true;
})
.OnError(exception =>
{
    Error(exception.Message);
    Information("Publish-Release-Packages Task failed, but continuing with next Task...");
    publishingError = true;
});


public void PushChocolateyPackages(ICakeContext context, bool isRelease, List<PackageSourceData> chocolateySources)
{
    if (BuildParameters.BuildAgentOperatingSystem == PlatformFamily.Windows && DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages))
    {
        Information("Number of configured {0} Chocolatey Sources: {1}", isRelease ? "Release" : "PreRelease", chocolateySources.Count());

        foreach (var chocolateySource in chocolateySources)
        {
            var nupkgFiles = GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*.nupkg");

            var chocolateyPushSettings = new ChocolateyPushSettings
                {
                    Source = chocolateySource.PushUrl
                };

            var canPushToChocolateySource = false;
            if (!string.IsNullOrEmpty(chocolateySource.Credentials.ApiKey))
            {
                context.Information("Setting ApiKey in Chocolatey Push Settings...");
                chocolateyPushSettings.ApiKey = chocolateySource.Credentials.ApiKey;
                canPushToChocolateySource = true;
            }
            else
            {
                if (!string.IsNullOrEmpty(chocolateySource.Credentials.User) && !string.IsNullOrEmpty(chocolateySource.Credentials.Password))
                {
                    var chocolateySourceSettings = new ChocolateySourcesSettings
                    {
                        UserName = chocolateySource.Credentials.User,
                        Password = chocolateySource.Credentials.Password
                    };

                    context.Information("Adding Chocolatey source with user/pass...");
                    context.ChocolateyAddSource(isRelease ? string.Format("ReleaseSource_{0}", chocolateySource.Name) : string.Format("PreReleaseSource_{0}", chocolateySource.Name), chocolateySource.PushUrl, chocolateySourceSettings);
                    canPushToChocolateySource = true;
                }
                else
                {
                    context.Warning("User and Password are missing for {0} Chocolatey Source with Url {1}", isRelease ? "Release" : "PreRelease", chocolateySource.PushUrl);
                }
            }

            if (canPushToChocolateySource)
            {
                foreach (var nupkgFile in nupkgFiles)
                {
                    context.Information("Pushing {0} to {1} Source with Url {2}...", nupkgFile, isRelease ? "Release" : "PreRelease", chocolateySource.PushUrl);

                    // Push the package.
                    context.ChocolateyPush(nupkgFile, chocolateyPushSettings);
                }
            }
            else
            {
                context.Warning("Unable to push Chocolatey Packages to {0} Source with Url {1} as necessary credentials haven't been provided.", isRelease ? "Release" : "PreRelease", chocolateySource.PushUrl);
            }
        }
    }
    else
    {
        context.Information("Unable to publish Chocolatey packages. IsRunningOnWindows: {0} Chocolatey Packages Directory Exists: {0}", BuildParameters.BuildAgentOperatingSystem, DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages));
    }
}
