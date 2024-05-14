// not loading analyzing.cake_ex here.

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.AnalyzeTask = Task("IntelliJAnalyze")
    .IsDependentOn("IntelliJBuild")
    .Does<BuildVersion>((context, buildVersion) =>
{
       Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithLogLevel(IntelliJBuildParameters.GradleVerbosity)
        .WithProjectProperty("pluginVersion", buildVersion.SemVersion)
        .WithTask("detekt")
        .WithTask("ktlintCheck")
        .WithTask("verifyPlugin")
        .Run();
});


IntelliJBuildParameters.Tasks.RunPluginVerifierTask = Task("Run-Plugin-Verifier")
    .IsDependentOn("IntelliJBuild")
    .WithCriteria(() => IntelliJBuildParameters.ShouldRunPluginVerifier, "Plugin Verifier is disabled")
    .Does<BuildVersion>((context, buildVersion) =>
{
       Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithLogLevel(IntelliJBuildParameters.GradleVerbosity)
        .WithProjectProperty("pluginVersion", buildVersion.SemVersion)
        .WithTask("runPluginVerifier")
        .Run();
});
