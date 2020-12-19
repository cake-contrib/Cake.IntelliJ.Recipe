public class BuildTasks
{
    public CakeTaskBuilder AnalyzeTask { get; set; }
    public CakeTaskBuilder RunPluginVerifierTask { get; set; }
    public CakeTaskBuilder PrintCiProviderEnvironmentVariablesTask { get; set; }
    public CakeTaskBuilder UploadArtifactsTask { get; set; }
    public CakeTaskBuilder ClearAppVeyorCacheTask { get; set; }
    public CakeTaskBuilder ShowInfoTask { get; set; }
    public CakeTaskBuilder CleanTask { get; set; }
    public CakeTaskBuilder RestoreTask { get; set; }
    public CakeTaskBuilder BuildTask { get; set; }
    public CakeTaskBuilder PackageTask { get; set; }
    public CakeTaskBuilder DefaultTask { get; set; }
    public CakeTaskBuilder ContinuousIntegrationTask { get; set; }
    public CakeTaskBuilder ReleaseNotesTask { get; set; }
    public CakeTaskBuilder LabelsTask { get; set; }
    public CakeTaskBuilder ClearCacheTask { get; set; }
    public CakeTaskBuilder PreviewTask { get; set; }
    public CakeTaskBuilder PublishDocsTask { get; set; }
    public CakeTaskBuilder CreateChocolateyPackagesTask { get; set; }
    public CakeTaskBuilder CreateReleaseNotesTask { get; set; }
    public CakeTaskBuilder ExportReleaseNotesTask { get; set; }
    public CakeTaskBuilder PublishGitHubReleaseTask { get; set; }
    public CakeTaskBuilder CreateDefaultLabelsTask { get; set; }
    public CakeTaskBuilder CreatePluginPackagesTask { get; set; }
    public CakeTaskBuilder PublishPreReleasePackagesTask { get; set; }
    public CakeTaskBuilder PublishReleasePackagesTask { get; set; }

    public CakeTaskBuilder TestTask { get; set; }
    public CakeTaskBuilder CleanDocumentationTask { get; set; }
    public CakeTaskBuilder PublishDocumentationTask { get; set; }
    public CakeTaskBuilder PreviewDocumentationTask { get; set; }
    public CakeTaskBuilder ForcePublishDocumentationTask { get; set; }
    public CakeTaskBuilder ReportMessagesToCi {get; set; }
}
