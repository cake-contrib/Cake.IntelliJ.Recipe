///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.TestTask = Task("Test")
    .IsDependentOn("Build")
    .Does<BuildVersion>((context, buildVersion) =>
{
     Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithLogLevel(BuildParameters.GradleVerbosity)
        .WithProjectProperty("pluginVersion", buildVersion.FullSemVersion)
        .WithTask("test")
        .Run(); 

});
