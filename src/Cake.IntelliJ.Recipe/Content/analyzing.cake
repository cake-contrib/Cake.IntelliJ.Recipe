///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.AnalyzeTask = Task("Analyze")
    .IsDependentOn("Build")
    .Does<BuildVersion>((context, buildVersion) => 
{
       Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithLogLevel(BuildParameters.GradleVerbosity)
        .WithProjectProperty("pluginVersion", buildVersion.SemVersion)
        .WithTask("detekt")
        .WithTask("ktlintCheck")
        .WithTask("verifyPlugin")
        .Run(); 
});


BuildParameters.Tasks.RunPluginVerifierTask = Task("Run-Plugin-Verifier")
    .IsDependentOn("Build")
    .Does<BuildVersion>((context, buildVersion) => 
{
       Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithLogLevel(BuildParameters.GradleVerbosity)
        .WithProjectProperty("pluginVersion", buildVersion.SemVersion)
        .WithTask("runPluginVerifier")
        .Run(); 
});
