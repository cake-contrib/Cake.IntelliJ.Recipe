///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.AnalyzeTask = Task("Analyze")
    .IsDependentOn("Build")
    .Does<BuildVersion>((context, buildVersion) => 
{
       Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithArguments($"-PpluginVersion=\"{buildVersion.SemVersion}\"") // workaround for cake.gradle implementing WithProperty("pluginVersion", "3.2.1")
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
        .WithArguments($"-PpluginVersion=\"{buildVersion.SemVersion}\"") // workaround for cake.gradle implementing WithProperty("pluginVersion", "3.2.1")
        .WithTask("runPluginVerifier")
        .Run(); 
});
