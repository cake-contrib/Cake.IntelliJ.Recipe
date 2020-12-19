///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

BuildParameters.Tasks.TestTask = Task("Test")
    .IsDependentOn("Build")
    .Does<BuildVersion>((context, buildVersion) =>
{
     Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithArguments($"-PpluginVersion=\"{buildVersion.SemVersion}\"") // workaround for cake.gradle implementing WithProperty("pluginVersion", "3.2.1")
        .WithTask("test")
        .Run(); 

});
