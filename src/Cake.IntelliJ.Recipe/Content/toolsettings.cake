public static class ToolSettings
{
    static ToolSettings()
    {
        SetToolPreprocessorDirectives();
    }

    public static int MaxCpuCount { get; private set; }

    public static string GitReleaseManagerTool { get; private set; }
    public static string GitVersionTool { get; private set; }
    public static string ReSharperTools { get; private set; }
    public static string KuduSyncTool { get; private set; }
    public static string WyamTool { get; private set; }
    public static string XUnitTool { get; private set; }
    public static string NUnitTool { get; private set; }
    public static string NuGetTool { get; private set; }
    public static string ReportGeneratorTool { get; private set; }
    public static string ReportUnitTool { get; private set; }

    public static string GitReleaseManagerGlobalTool { get; private set; }
    public static string GitVersionGlobalTool { get; private set; }
    public static string ReportGeneratorGlobalTool { get; private set; }
    public static string WyamGlobalTool { get; private set; }
    public static string KuduSyncGlobalTool { get; private set; }

    public static void SetToolPreprocessorDirectives(
        string gitReleaseManagerTool = "#tool nuget:?package=GitReleaseManager&version=0.11.0",
        // This is specifically pinned to 5.0.1 as later versions break compatibility with Unix.
        string gitVersionTool = "#tool nuget:?package=GitVersion.CommandLine&version=5.0.1",
        string reSharperTools = "#tool nuget:?package=JetBrains.ReSharper.CommandLineTools&version=2020.2.4",
        string kuduSyncTool = "#tool nuget:?package=KuduSync.NET&version=1.5.3",
        string wyamTool = "#tool nuget:?package=Wyam&version=2.2.9",
        string xunitTool = "#tool nuget:?package=xunit.runner.console&version=2.4.1",
        string nunitTool = "#tool nuget:?package=NUnit.ConsoleRunner&version=3.11.1",
        string nugetTool = "#tool nuget:?package=NuGet.CommandLine&version=5.7.0",
        string reportGeneratorTool = "#tool nuget:?package=ReportGenerator&version=4.7.1",
        string reportUnitTool = "#tool nuget:?package=ReportUnit&version=1.2.1",
        string gitReleaseManagerGlobalTool = "#tool dotnet:?package=GitReleaseManager.Tool&version=0.11.0",
        string gitVersionGlobalTool = "#tool dotnet:?package=GitVersion.Tool&version=5.5.1",
        string reportGeneratorGlobalTool = "#tool dotnet:?package=dotnet-reportgenerator-globaltool&version=4.7.1",
        string wyamGlobalTool = "#tool dotnet:?package=Wyam.Tool&version=2.2.9",
        // This is using an unofficial build of kudusync so that we can have a .Net Global tool version.  This was generated from this PR: https://github.com/projectkudu/KuduSync.NET/pull/27
        string kuduSyncGlobalTool = "#tool dotnet:https://www.myget.org/F/cake-contrib/api/v3/index.json?package=KuduSync.Tool&version=1.5.4-g3916ad7218"
    )
    {
        GitReleaseManagerTool = gitReleaseManagerTool;
        GitVersionTool = gitVersionTool;
        ReSharperTools = reSharperTools;
        KuduSyncTool = kuduSyncTool;
        WyamTool = wyamTool;
        XUnitTool = xunitTool;
        NUnitTool = nunitTool;
        NuGetTool = nugetTool;
        ReportGeneratorTool = reportGeneratorTool;
        ReportUnitTool = reportUnitTool;
        ReportGeneratorGlobalTool = reportGeneratorGlobalTool;
        GitVersionGlobalTool = gitVersionGlobalTool;
        GitReleaseManagerGlobalTool = gitReleaseManagerGlobalTool;
        WyamGlobalTool = wyamGlobalTool;
        KuduSyncGlobalTool = kuduSyncGlobalTool;
    }

    public static void SetToolSettings(
        ICakeContext context,
        int? maxCpuCount = null
    )
    {
        context.Information("Setting up tools...");

        MaxCpuCount = maxCpuCount ?? 0;
    }
}
