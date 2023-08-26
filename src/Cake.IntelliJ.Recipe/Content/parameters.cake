#l ./Cake.Recipe/parameters.cake_ex

public static class IntelliJBuildParameters
{
    const string StandardMessage = "Version {0} of the {1} plugin has just been released, this will be available here https://plugins.jetbrains.com/plugin/{2}, once the version is reviewed and approved.";
    public static string MarketplaceId { get; private set; }
    public static GradleLogLevel GradleVerbosity { get; private set; }
    public static string PluginReleaseChannel { get; private set; }
    public static string PluginPreReleaseChannel { get; private set; }
    public static string PluginCiBuildChannel { get; private set; }
    public static bool ShouldPublishPluginCiBuilds { get; private set; }
    public static string PluginChannelGradleProperty { get; private set; }
    public static string PluginVersionGradleProperty { get; private set; }
    public static DirectoryPath PluginBuildOutputPath { get; private set; }
    public static DirectoryPath PluginPackOutputPath { get; private set; }
    public static FilePath IntegrationTestScriptPath { get; private set; }
    public static IntelliJBuildTasks Tasks { get; private set; }
    public static IntelliJBuildPaths Paths { get; private set; }
    static IntelliJBuildParameters()
    {
        Tasks = new IntelliJBuildTasks();
    }

    public static void PrintParameters(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        BuildParameters.PrintParameters(context);

        context.Information("MarketplaceId: {0}", MarketplaceId);
        context.Information("GradleVerbosity: {0}", Enum.GetName(typeof(GradleLogLevel), GradleVerbosity));
        context.Information("PluginReleaseChannel: {0}", PluginReleaseChannel);
        context.Information("PluginPreReleaseChannel: {0}", PluginPreReleaseChannel);
        context.Information("PluginCiBuildChannel: {0}", PluginCiBuildChannel);
        context.Information("ShouldPublishPluginCiBuilds: {0}", ShouldPublishPluginCiBuilds);
        context.Information("PluginChannelGradleProperty: {0}", PluginChannelGradleProperty);
        context.Information("PluginVersionGradleProperty: {0}", PluginVersionGradleProperty);
    }

    public static void SetParameters(
        ICakeContext context,
        BuildSystem buildSystem,
        DirectoryPath sourceDirectoryPath,
        string title,
        FilePath solutionFilePath = null,
        DirectoryPath rootDirectoryPath = null,
        DirectoryPath testDirectoryPath = null,
        string testFilePattern = null,
        string integrationTestScriptPath = null,
        string resharperSettingsFileName = null,
        string repositoryOwner = null,
        string repositoryName = null,
        string appVeyorAccountName = null,
        string appVeyorProjectSlug = null,
        bool shouldPostToGitter = true,
        bool shouldPostToSlack = true,
        bool shouldPostToTwitter = true,
        bool shouldPostToMicrosoftTeams = false,
        bool shouldSendEmail = true,
        bool shouldDownloadMilestoneReleaseNotes = false,
        bool shouldDownloadFullReleaseNotes = false,
        bool shouldNotifyBetaReleases = false,
        bool shouldDeleteCachedFiles = false,
        bool shouldUseDeterministicBuilds = true,
        FilePath milestoneReleaseNotesFilePath = null,
        FilePath fullReleaseNotesFilePath = null,
        bool shouldRunChocolatey = true,
        bool shouldPublishGitHub = true,
        bool shouldGenerateDocumentation = true,
        bool shouldDocumentSourceFiles = true,
        bool shouldRunInspectCode = false,
        bool shouldRunCoveralls = false,
        bool shouldRunCodecov = false,
        bool shouldRunDotNetCorePack = false,
        bool shouldBuildNugetSourcePackage = false,
        bool shouldRunIntegrationTests = false,
        bool shouldCalculateVersion = true,
        bool? shouldUseTargetFrameworkPath = null,
        bool? transifexEnabled = null,
        TransifexMode transifexPullMode = TransifexMode.OnlyTranslated,
        int transifexPullPercentage = 60,
        string gitterMessage = null,
        string microsoftTeamsMessage = null,
        string twitterMessage = null,
        DirectoryPath wyamRootDirectoryPath = null,
        DirectoryPath wyamPublishDirectoryPath = null,
        FilePath wyamConfigurationFile = null,
        string wyamRecipe = null,
        string wyamTheme = null,
        string wyamSourceFiles = null,
        string webHost = null,
        string webLinkRoot = null,
        string webBaseEditUrl = null,
        FilePath nuspecFilePath = null,
        bool isPublicRepository = true,
        FilePath nugetConfig = null,
        ICollection<string> nuGetSources = null,
        bool treatWarningsAsErrors = true,
        string masterBranchName = "master",
        string developBranchName = "develop",
        string emailRecipient = null,
        string emailSenderName = null,
        string emailSenderAddress = null,
        DirectoryPath restorePackagesDirectory = null,
        List<PackageSourceData> packageSourceDatas = null,
        PlatformFamily preferredBuildAgentOperatingSystem = PlatformFamily.Windows,
        BuildProviderType preferredBuildProviderType = BuildProviderType.AppVeyor,

        string pluginReleaseChannel = "Stable",
        string pluginPreReleaseChannel = "Beta",
        string pluginCiBuildChannel = "Alpha",
        bool shouldPublishPluginCiBuilds = false,
        string pluginChannelGradleProperty = "marketplaceChannel",
        string pluginVersionGradleProperty = "pluginVersion",
        string marketplaceId = null,
        GradleLogLevel gradleVerbosity = GradleLogLevel.Default,
        DirectoryPath pluginBuildOutputPath = null,
        DirectoryPath pluginPackOutputPath = null
        )
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        if(string.IsNullOrEmpty(marketplaceId))
        {
            marketplaceId = title; // TODO: this is not a good default..
        }

        if(string.IsNullOrEmpty(gitterMessage))
        {
            gitterMessage = "@/all " + StandardMessage.Replace("{2}", marketplaceId);
        }

        if(string.IsNullOrEmpty(microsoftTeamsMessage))
        {
            microsoftTeamsMessage = StandardMessage.Replace("{2}", marketplaceId);
        }

        if(string.IsNullOrEmpty(twitterMessage))
        {
            twitterMessage = StandardMessage.Replace("{2}", marketplaceId);
        }

        BuildParameters.SetParameters(
            context: context,
            buildSystem: buildSystem,
            sourceDirectoryPath: sourceDirectoryPath,
            title: title,
            solutionFilePath: solutionFilePath,
            rootDirectoryPath: rootDirectoryPath,
            testDirectoryPath: testDirectoryPath,
            testFilePattern: testFilePattern,
            integrationTestScriptPath: integrationTestScriptPath,
            resharperSettingsFileName: resharperSettingsFileName,
            repositoryOwner: repositoryOwner,
            repositoryName: repositoryName,
            appVeyorAccountName: appVeyorAccountName,
            appVeyorProjectSlug: appVeyorProjectSlug,
            shouldPostToGitter: shouldPostToGitter,
            shouldPostToSlack: shouldPostToSlack,
            shouldPostToTwitter: shouldPostToTwitter,
            shouldPostToMicrosoftTeams: shouldPostToMicrosoftTeams,
            shouldSendEmail: shouldSendEmail,
            shouldDownloadMilestoneReleaseNotes: shouldDownloadMilestoneReleaseNotes,
            shouldDownloadFullReleaseNotes: shouldDownloadFullReleaseNotes,
            shouldNotifyBetaReleases: shouldNotifyBetaReleases,
            shouldDeleteCachedFiles: shouldDeleteCachedFiles,
            shouldUseDeterministicBuilds: shouldUseDeterministicBuilds,
            milestoneReleaseNotesFilePath: milestoneReleaseNotesFilePath,
            fullReleaseNotesFilePath: fullReleaseNotesFilePath,
            shouldRunChocolatey: shouldRunChocolatey,
            shouldPublishGitHub: shouldPublishGitHub,
            shouldGenerateDocumentation: shouldGenerateDocumentation,
            shouldDocumentSourceFiles: shouldDocumentSourceFiles,
            shouldRunInspectCode: shouldRunInspectCode,
            shouldRunCoveralls: shouldRunCoveralls,
            shouldRunCodecov: shouldRunCodecov,
            shouldRunDotNetCorePack: shouldRunDotNetCorePack,
            shouldBuildNugetSourcePackage: shouldBuildNugetSourcePackage,
            shouldRunIntegrationTests: shouldRunIntegrationTests,
            shouldCalculateVersion: shouldCalculateVersion,
            shouldUseTargetFrameworkPath: shouldUseTargetFrameworkPath,
            transifexEnabled: transifexEnabled,
            transifexPullMode: transifexPullMode,
            transifexPullPercentage: transifexPullPercentage,
            gitterMessage: gitterMessage,
            microsoftTeamsMessage: microsoftTeamsMessage,
            twitterMessage: twitterMessage,
            wyamRootDirectoryPath: wyamRootDirectoryPath,
            wyamPublishDirectoryPath: wyamPublishDirectoryPath,
            wyamConfigurationFile: wyamConfigurationFile,
            wyamRecipe: wyamRecipe,
            wyamTheme: wyamTheme,
            wyamSourceFiles: wyamSourceFiles,
            webHost: webHost,
            webLinkRoot: webLinkRoot,
            webBaseEditUrl: webBaseEditUrl,
            nuspecFilePath: nuspecFilePath,
            isPublicRepository: isPublicRepository,
            nugetConfig: nugetConfig,
            nuGetSources: nuGetSources,
            treatWarningsAsErrors: treatWarningsAsErrors,
            masterBranchName: masterBranchName,
            developBranchName: developBranchName,
            emailRecipient: emailRecipient,
            emailSenderName: emailSenderName,
            emailSenderAddress: emailSenderAddress,
            restorePackagesDirectory: restorePackagesDirectory,
            packageSourceDatas: packageSourceDatas,
            preferredBuildAgentOperatingSystem: preferredBuildAgentOperatingSystem,
            preferredBuildProviderType: preferredBuildProviderType
        );

        PluginBuildOutputPath = context.MakeAbsolute(pluginBuildOutputPath ?? (sourceDirectoryPath + "/build/libs"));
        PluginPackOutputPath = context.MakeAbsolute(pluginPackOutputPath ?? (sourceDirectoryPath + "/build/distributions"));
        MarketplaceId = marketplaceId;
        GradleVerbosity = gradleVerbosity;
        PluginReleaseChannel = pluginReleaseChannel;
        PluginPreReleaseChannel = pluginPreReleaseChannel;
        PluginCiBuildChannel = pluginCiBuildChannel;
        ShouldPublishPluginCiBuilds = shouldPublishPluginCiBuilds;
        PluginChannelGradleProperty = pluginChannelGradleProperty;
        PluginVersionGradleProperty = pluginVersionGradleProperty;
        Paths = IntelliJBuildPaths.GetPaths(context);
    }
}
