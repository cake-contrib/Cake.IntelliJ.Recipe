#load nuget:?package=Cake.Recipe&version=2.2.0

var standardNotificationMessage = "Version {0} of {1} has just been released, it will be available here https://www.nuget.org/packages/{1}, once package indexing is complete.";

Environment.SetVariableNames();

BuildParameters.SetParameters(
    context: Context,
    buildSystem: BuildSystem,
    masterBranchName: "main",
    sourceDirectoryPath: "./src",
    title: "Cake.IntelliJ.Recipe",
    repositoryName: "Cake.IntelliJ.Recipe", // workaround for https://github.com/cake-contrib/Cake.Recipe/issues/687
    repositoryOwner: "cake-contrib",
    shouldRunInspectCode: false,
    shouldRunDupFinder: false,
    shouldRunIntegrationTests: false,
    shouldRunCoveralls: false,
    shouldRunCodecov: false,
    shouldRunDotNetCorePack: true,
    gitterMessage: "@/all " + standardNotificationMessage,
    twitterMessage: standardNotificationMessage);

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context);

BuildParameters.Tasks.CleanTask
    .IsDependentOn("Generate-Version-File");

Task("Generate-Version-File")
    .Does<BuildVersion>((context, buildVersion) => {
        var buildMetaDataCodeGen = TransformText(@"
        public class BuildMetaData
        {
            public static string Date { get; } = ""<%date%>"";
            public static string Version { get; } = ""<%version%>"";
            public static string CakeVersion { get; } = ""<%cakeVersion%>"";
        }",
        "<%",
        "%>"
        )
   .WithToken("date", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
   .WithToken("version", buildVersion.SemVersion)
   .WithToken("cakeVersion", context.GetType().Assembly.GetName().Version)
   .ToString();

    System.IO.File.WriteAllText(
        "./src/Cake.IntelliJ.Recipe/Content/version.cake",
        buildMetaDataCodeGen
    );
});

Build.RunDotNetCore();
