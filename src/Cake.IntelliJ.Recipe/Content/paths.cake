public class BuildPaths
{
    public BuildFiles Files { get; private set; }
    public BuildDirectories Directories { get; private set; }

    public static BuildPaths GetPaths(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }

        // Directories
        var buildDirectoryPath             = "./BuildArtifacts";
        var tempBuildDirectoryPath         = buildDirectoryPath + "/temp";
        var publishedWebsitesDirectory     = tempBuildDirectoryPath + "/_PublishedWebsites";
        var publishedApplicationsDirectory = tempBuildDirectoryPath + "/_PublishedApplications";
        var publishedLibrariesDirectory    = tempBuildDirectoryPath + "/_PublishedLibraries";
        var backupBuildXmlDirectory        = tempBuildDirectoryPath + "/_BackupBuildXml";
        var publishedDocumentationDirectory= buildDirectoryPath + "/Documentation";

        var chocolateyNuspecDirectory = "./nuspec/chocolatey";

        var testResultsDirectory = buildDirectoryPath + "/TestResults";

        var packagesDirectory = buildDirectoryPath + "/Packages";
        var pluginPackagesOutputDirectory = packagesDirectory + "/Plugin";
        var chocolateyPackagesOutputDirectory = packagesDirectory + "/Chocolatey";

        // Files
        var solutionInfoFilePath = ((DirectoryPath)BuildParameters.SourceDirectoryPath).CombineWithFilePath("SolutionInfo.cs");
        var buildBinLogFilePath = ((DirectoryPath)buildDirectoryPath).CombineWithFilePath("build.binlog");

        var repoFilesPaths = new FilePath[] {
            "LICENSE",
            "README.md"
        };

        var buildDirectories = new BuildDirectories(
            buildDirectoryPath,
            tempBuildDirectoryPath,
            publishedWebsitesDirectory,
            publishedApplicationsDirectory,
            publishedLibrariesDirectory,
            publishedDocumentationDirectory,
            chocolateyNuspecDirectory,
            testResultsDirectory,
            pluginPackagesOutputDirectory,
            chocolateyPackagesOutputDirectory,
            packagesDirectory,
            backupBuildXmlDirectory
            );

        var buildFiles = new BuildFiles(
            context,
            repoFilesPaths,
            solutionInfoFilePath,
            buildBinLogFilePath
        );

        return new BuildPaths
        {
            Files = buildFiles,
            Directories = buildDirectories
        };
    }
}

public class BuildFiles
{
    public ICollection<FilePath> RepoFilesPaths { get; private set; }

    public FilePath SolutionInfoFilePath { get; private set; }

    public FilePath BuildBinLogFilePath { get; private set; }

    public BuildFiles(
        ICakeContext context,
        FilePath[] repoFilesPaths,
        FilePath solutionInfoFilePath,
        FilePath buildBinLogFilePath
        )
    {
        RepoFilesPaths = Filter(context, repoFilesPaths);
        SolutionInfoFilePath = solutionInfoFilePath;
        BuildBinLogFilePath = buildBinLogFilePath;
    }

    private static FilePath[] Filter(ICakeContext context, FilePath[] files)
    {
        // Not a perfect solution, but we need to filter PDB files
        // when building on an OS that's not Windows (since they don't exist there).

        if (BuildParameters.BuildAgentOperatingSystem != PlatformFamily.Windows)
        {
            return files.Where(f => !f.FullPath.EndsWith("pdb")).ToArray();
        }

        return files;
    }
}

public class BuildDirectories
{
    public DirectoryPath Build { get; private set; }
    public DirectoryPath TempBuild { get; private set; }
    public DirectoryPath PublishedWebsites { get; private set; }
    public DirectoryPath PublishedApplications { get; private set; }
    public DirectoryPath PublishedLibraries { get; private set; }
    public DirectoryPath PublishedDocumentation { get; private set; }
    public DirectoryPath ChocolateyNuspecDirectory { get; private set; }
    public DirectoryPath TestResults { get; private set; }
    public DirectoryPath PluginPackages { get; private set; }
    public DirectoryPath ChocolateyPackages { get; private set; }
    public DirectoryPath Packages { get; private set; }
    public DirectoryPath BackupBuildXml { get; private set; }
    public ICollection<DirectoryPath> ToClean { get; private set; }

    public BuildDirectories(
        DirectoryPath build,
        DirectoryPath tempBuild,
        DirectoryPath publishedWebsites,
        DirectoryPath publishedApplications,
        DirectoryPath publishedLibraries,
        DirectoryPath publishedDocumentation,
        DirectoryPath chocolateyNuspecDirectory,
        DirectoryPath testResults,
        DirectoryPath pluginPackages,
        DirectoryPath chocolateyPackages,
        DirectoryPath packages,
        DirectoryPath backupBuildXmlDirectory
        )
    {
        Build = build;
        TempBuild = tempBuild;
        PublishedWebsites = publishedWebsites;
        PublishedApplications = publishedApplications;
        PublishedLibraries = publishedLibraries;
        PublishedDocumentation = publishedDocumentation;
        ChocolateyNuspecDirectory = chocolateyNuspecDirectory;
        TestResults = testResults;
        PluginPackages = pluginPackages;
        ChocolateyPackages = chocolateyPackages;
        Packages = packages;
        BackupBuildXml = backupBuildXmlDirectory;

        ToClean = new[] {
            Build,
            TempBuild,
            TestResults
        };
    }
}
