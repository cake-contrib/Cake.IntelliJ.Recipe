#l ./Cake.Recipe/paths.cake_ex

public class IntelliJBuildPaths
{
    public IntelliJBuildDirectories Directories { get; private set; }

    public static IntelliJBuildPaths GetPaths(ICakeContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException("context");
        }
        var basePaths = BuildPaths.GetPaths(context);

        // Directories
        var backupBuildXmlDirectory        = basePaths.Directories.TempBuild + "/_BackupBuildXml";
        var pluginPackagesOutputDirectory = basePaths.Directories.Packages + "/Plugin";

        var buildDirectories = new IntelliJBuildDirectories(
            pluginPackagesOutputDirectory,
            backupBuildXmlDirectory
        );


        return new IntelliJBuildPaths
        {
            Directories = buildDirectories
        };
    }
}

public class IntelliJBuildDirectories
{
    public DirectoryPath BackupBuildXml { get; private set; }
    public DirectoryPath PluginPackages { get; private set; }

    public IntelliJBuildDirectories(
        DirectoryPath pluginPackagesOutputDirectory,
        DirectoryPath backupBuildXmlDirectory
        )
    {
        BackupBuildXml = backupBuildXmlDirectory;
        PluginPackages = pluginPackagesOutputDirectory;
    }
}
