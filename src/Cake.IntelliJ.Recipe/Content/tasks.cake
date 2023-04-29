#l ./Cake.Recipe/tasks.cake_ex

public class IntelliJBuildTasks
{
    public CakeTaskBuilder RunPluginVerifierTask { get; set; }

    public CakeTaskBuilder PrintJavaEnvironmentVariablesTask { get; set; }
    
    public CakeTaskBuilder CreatePluginPackagesTask { get; set; }
    public CakeTaskBuilder ForcePublishPlugin { get; set; }
}
