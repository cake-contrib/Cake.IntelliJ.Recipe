#l ./Cake.Recipe/testing.cake_ex

BuildParameters.Tasks.TestTask = Task("IntelliJTest")
    .IsDependentOn("IntelliJBuild")
    .Does<BuildVersion>((context, buildVersion) =>
{
     Gradle
        .FromPath(BuildParameters.SourceDirectoryPath)
        .WithLogLevel(IntelliJBuildParameters.GradleVerbosity)
        .WithProjectProperty("pluginVersion", buildVersion.SemVersion)
        .WithTask("test")
        .Run(); 

});
