#l ./Cake.Recipe/build.cake_ex

///////////////////////////////////////////////////////////////////////////////
// TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Teardown((context) =>
{
    if (!BuildParameters.IsLocalBuild)
    {
        // stop all gradle daemons, or else the Ci might run indefinitely
        Information("Stopping gradle daemons...");
        Gradle
            .FromPath(BuildParameters.SourceDirectoryPath)
            .WithLogLevel(IntelliJBuildParameters.GradleVerbosity)
            .WithArguments("--stop")
            .Run();
    }
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////


BuildParameters.Tasks.CleanTask.IsDependentOn("IntelliJClean");
Task("IntelliJClean")
    .IsDependentOn("Print-Java-Environment-Variables")
    .Does(() =>
{
    Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithLogLevel(IntelliJBuildParameters.GradleVerbosity)
        .WithTask("clean")
        .Run();
});

BuildParameters.Tasks.RestoreTask.WithCriteria(false, "IntelliJ");

BuildParameters.Tasks.BuildTask.WithCriteria(false, "IntelliJ").IsDependentOn("IntelliJBuild");
BuildParameters.Tasks.BuildTask = Task("IntelliJBuild")
    .IsDependentOn("IntelliJClean")
    .Does<BuildVersion>((context, buildVersion) =>
{
    Information("Building {0} for version {1}", BuildParameters.SourceDirectoryPath, buildVersion.SemVersion);

    Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithLogLevel(IntelliJBuildParameters.GradleVerbosity)
        .WithTask("build")
        .WithProjectProperty("pluginVersion", buildVersion.SemVersion)
        .Run();

    // copy jar to output
    var outputFolder = BuildParameters.Paths.Directories.PublishedLibraries;
    EnsureDirectoryExists(outputFolder);
    var files = GetFiles(IntelliJBuildParameters.PluginBuildOutputPath + "/**/*");
    if (files.Any())
    {
        CopyFiles(files, outputFolder, true);
    }
    else
    {
        Warning("No files were found in the build output directory: '{0}'", IntelliJBuildParameters.PluginBuildOutputPath);
    }
});

BuildParameters.Tasks.PackageTask.WithCriteria(false, "IntelliJ").IsDependentOn("IntelliJPackage");
BuildParameters.Tasks.PackageTask = Task("IntelliJPackage")
    .IsDependeeOf("Package")
    .IsDependentOn("Export-Release-Notes");

BuildParameters.Tasks.DefaultTask.IsDependentOn("IntelliJPackage");


///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

public IntelliJBuilder IntelliJBuild
{
    get
    {
        return new IntelliJBuilder(target => RunTarget(target));
    }
}

public class IntelliJBuilder
{
    private Action<string> _action;

    public IntelliJBuilder(Action<string> action)
    {
        _action = action;
    }

    public void Run(bool useNetCoreTools = true)
    {
        BuildParameters.IsDotNetCoreBuild = useNetCoreTools;
        BuildParameters.IsNuGetBuild = false;

        SetupTasks();

        _action(BuildParameters.Target);
    }


    private static void SetupTasks()
    {
        BuildParameters.Tasks.PackageTask.IsDependentOn("IntelliJAnalyze");
        BuildParameters.Tasks.PackageTask.IsDependentOn("IntelliJTest");
        BuildParameters.Tasks.PackageTask.IsDependentOn("Create-Plugin-Packages");
        if(!BuildParameters.IsLocalBuild) {
            BuildParameters.Tasks.PackageTask.IsDependentOn("Run-Plugin-Verifier");
        }
    }
}
