///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var publishingError = false;

///////////////////////////////////////////////////////////////////////////////
// Support function for comparing cake version support
///////////////////////////////////////////////////////////////////////////////
public bool IsSupportedCakeVersion(string supportedVersion, string currentVersion)
{
    var twoPartSupported = Version.Parse(supportedVersion).ToString(2);
    var twoPartCurrent = Version.Parse(currentVersion).ToString(2);

    return twoPartCurrent == twoPartSupported;
}

///////////////////////////////////////////////////////////////////////////////
// TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Teardown<BuildVersion>((context, buildVersion) =>
{
    Information("Starting Teardown...");

    if (BuildParameters.PublishReleasePackagesWasSuccessful)
    {
        if (!BuildParameters.IsLocalBuild &&
            !BuildParameters.IsPullRequest &&
            BuildParameters.IsMainRepository &&
            (BuildParameters.BranchType == BranchType.Master ||
                ((BuildParameters.BranchType == BranchType.Release || BuildParameters.BranchType == BranchType.HotFix) &&
                BuildParameters.ShouldNotifyBetaReleases)) &&
            BuildParameters.IsTagged)
        {
            if (BuildParameters.CanPostToTwitter && BuildParameters.ShouldPostToTwitter)
            {
                SendMessageToTwitter(string.Format(BuildParameters.TwitterMessage, buildVersion.Version, BuildParameters.Title));
            }

            if (BuildParameters.CanPostToGitter && BuildParameters.ShouldPostToGitter)
            {
                SendMessageToGitterRoom(string.Format(BuildParameters.GitterMessage, buildVersion.Version, BuildParameters.Title));
            }

            if (BuildParameters.CanPostToMicrosoftTeams && BuildParameters.ShouldPostToMicrosoftTeams)
            {
                SendMessageToMicrosoftTeams(string.Format(BuildParameters.MicrosoftTeamsMessage, buildVersion.Version, BuildParameters.Title));
            }

            if (BuildParameters.CanSendEmail && BuildParameters.ShouldSendEmail && !string.IsNullOrEmpty(BuildParameters.EmailRecipient))
            {
                var subject = $"Continuous Integration Build of {BuildParameters.Title} completed successfully";
                var message = new StringBuilder();
                message.AppendLine(string.Format(BuildParameters.StandardMessage, buildVersion.Version, BuildParameters.Title) + "<br/>");
                message.AppendLine("<br/>");
                message.AppendLine($"<strong>Name</strong>: {BuildParameters.Title}<br/>");
                message.AppendLine($"<strong>Version</strong>: {buildVersion.SemVersion}<br/>");
                message.AppendLine($"<strong>Configuration</strong>: {BuildParameters.Configuration}<br/>");
                message.AppendLine($"<strong>Target</strong>: {BuildParameters.Target}<br/>");
                message.AppendLine($"<strong>Cake version</strong>: {buildVersion.CakeVersion}<br/>");
                message.AppendLine($"<strong>Cake.Recipe version</strong>: {BuildMetaData.Version}<br/>");

                SendEmail(subject, message.ToString(), BuildParameters.EmailRecipient, BuildParameters.EmailSenderName, BuildParameters.EmailSenderAddress);
            }
        }
    }

    if(!context.Successful)
    {
        if (!BuildParameters.IsLocalBuild &&
            BuildParameters.IsMainRepository)
        {
            if (BuildParameters.CanPostToSlack && BuildParameters.ShouldPostToSlack)
            {
                SendMessageToSlackChannel("Continuous Integration Build of " + BuildParameters.Title + " just failed :-(");
            }

            if (BuildParameters.CanSendEmail && BuildParameters.ShouldSendEmail && !string.IsNullOrEmpty(BuildParameters.EmailRecipient))
            {
                var subject = $"Continuous Integration Build of {BuildParameters.Title} failed";
                var message = context.ThrownException.ToString().Replace(System.Environment.NewLine, "<br/>");

                SendEmail(subject, message, BuildParameters.EmailRecipient, BuildParameters.EmailSenderName, BuildParameters.EmailSenderAddress);
            }
        }
    }

    // Clear nupkg files from tools directory
    if ((!BuildParameters.IsLocalBuild || BuildParameters.ShouldDeleteCachedFiles) && DirectoryExists(Context.Environment.WorkingDirectory.Combine("tools")))
    {
        Information("Deleting nupkg files...");
        var nupkgFiles = GetFiles(Context.Environment.WorkingDirectory.Combine("tools") + "/**/*.nupkg");
        DeleteFiles(nupkgFiles);
    }

    Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.ShowInfoTask = Task("Show-Info")
    .Does(() =>
{
    Information("Build Platform: {0}", BuildParameters.Platform);
    Information("Target: {0}", BuildParameters.Target);
    Information("Configuration: {0}", BuildParameters.Configuration);
    Information("PrepareLocalRelease: {0}", BuildParameters.PrepareLocalRelease);
    Information("ShouldDownloadMilestoneReleaseNotes: {0}", BuildParameters.ShouldDownloadMilestoneReleaseNotes);
    Information("ShouldDownloadFullReleaseNotes: {0}", BuildParameters.ShouldDownloadFullReleaseNotes);
    Information("IsLocalBuild: {0}", BuildParameters.IsLocalBuild);
    Information("IsPullRequest: {0}", BuildParameters.IsPullRequest);
    Information("IsMainRepository: {0}", BuildParameters.IsMainRepository);
    Information("IsTagged: {0}", BuildParameters.IsTagged);

    Information("Source DirectoryPath: {0}", MakeAbsolute(BuildParameters.SourceDirectoryPath));
    Information("Build DirectoryPath: {0}", MakeAbsolute(BuildParameters.Paths.Directories.Build));
});

BuildParameters.Tasks.CleanTask = Task("Clean")
    .IsDependentOn("Show-Info")
    .IsDependentOn("Print-CI-Provider-Environment-Variables")
    .Does(() =>
{
    Information("Cleaning...");

    CleanDirectories(BuildParameters.Paths.Directories.ToClean);
    Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithTask("clean")
        .Run(); 
});

BuildParameters.Tasks.RestoreTask = Task("Restore")
    .Does(() =>
{
    Information("Nothing to restore here. I think.");
});

BuildParameters.Tasks.BuildTask = Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Export-Release-Notes")
    .Does<BuildVersion>((context, buildVersion) => 
{
    Information("Building {0} for version {1}", BuildParameters.SourceDirectoryPath, buildVersion.SemVersion);

    // MODIFY SETTINGS releasenotes
    Warning("ReleaseNotes are missing!"); // // ReleaseNotes = BuildParameters.ReleaseNotes.Notes.ToArray(), ??
    Warning(BuildParameters.FullReleaseNotesFilePath);


     Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithTask("build")
        .WithArguments($"-PpluginVersion=\"{buildVersion.SemVersion}\"") // workaround for cake.gradle implementing WithProperty("pluginVersion", "3.2.1")
        .Run();

    // copy jar to output
    var outputFolder = BuildParameters.Paths.Directories.PublishedLibraries;
    EnsureDirectoryExists(outputFolder);
    var files = GetFiles(BuildParameters.PluginBuildOutputPath + "/**/*");
    if (files.Any())
    {
        CopyFiles(files, outputFolder, true);
    }
    else
    {
        Warning("No files were found in the build output directory: '{0}'", BuildParameters.PluginBuildOutputPath);
    }
});



BuildParameters.Tasks.PackageTask = Task("Package")
    .IsDependentOn("Export-Release-Notes");

BuildParameters.Tasks.DefaultTask = Task("Default")
    .IsDependentOn("Package")
    // Run issues task from Cake.Issues.Recipe by default.
    .IsDependentOn("Issues");

BuildParameters.Tasks.UploadArtifactsTask = Task("Upload-Artifacts")
    .IsDependentOn("Package")
    .WithCriteria(() => !BuildParameters.IsLocalBuild)
    .WithCriteria(() => DirectoryExists(BuildParameters.Paths.Directories.PluginPackages) || DirectoryExists(BuildParameters.Paths.Directories.ChocolateyPackages))
    .Does(() =>
{
    var artifacts = GetFiles(BuildParameters.Paths.Directories.Packages + "/*") +
                           GetFiles(BuildParameters.Paths.Directories.PluginPackages + "/*") +
                           GetFiles(BuildParameters.Paths.Directories.ChocolateyPackages + "/*");

      foreach (var artifact in artifacts)
    {
        BuildParameters.BuildProvider.UploadArtifact(artifact);
    }
});

BuildParameters.Tasks.ContinuousIntegrationTask = Task("CI")
    // Run issues task from Cake.Issues.Recipe by default.
    .IsDependentOn("Upload-Artifacts")
    .IsDependentOn("Issues")
    .IsDependentOn("Publish-PreRelease-Packages")
    .IsDependentOn("Publish-Release-Packages")
    .IsDependentOn("Publish-GitHub-Release")
    .IsDependentOn("Publish-Documentation")
    .Finally(() =>
{
    if (publishingError)
    {
        throw new Exception("An error occurred during the publishing of " + BuildParameters.Title + ".  All publishing tasks have been attempted.");
    }
});

BuildParameters.Tasks.ReleaseNotesTask = Task("ReleaseNotes")
  .IsDependentOn("Create-Release-Notes");

BuildParameters.Tasks.LabelsTask = Task("Labels")
  .IsDependentOn("Create-Default-Labels");

BuildParameters.Tasks.ClearCacheTask = Task("ClearCache")
  .IsDependentOn("Clear-AppVeyor-Cache");

BuildParameters.Tasks.PreviewTask = Task("Preview")
  .IsDependentOn("Preview-Documentation");

BuildParameters.Tasks.PublishDocsTask = Task("PublishDocs")
    .IsDependentOn("Force-Publish-Documentation");

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

public Builder Build
{
    get
    {
        return new Builder(target => RunTarget(target));
    }
}

public class Builder
{
    private Action<string> _action;

    public Builder(Action<string> action)
    {
        _action = action;
    }

    public void Run()
    {
        SetupTasks();

        _action(BuildParameters.Target);
    }

    
    private static void SetupTasks()
    {
        BuildParameters.Tasks.CreateChocolateyPackagesTask.IsDependentOn("Build");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Analyze");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Test");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-Chocolatey-Packages");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-Plugin-Packages");
        if(!BuildParameters.IsLocalBuild) {
            BuildParameters.Tasks.PackageTask.IsDependentOn("Run-Plugin-Verifier");
        }
    }
}