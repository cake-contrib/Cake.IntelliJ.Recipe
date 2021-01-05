BuildParameters.Tasks.PrintJavaEnvironmentVariablesTask = Task("Print-Java-Environment-Variables")
    .Does((context) => {
        var variables = new[] {
            "JAVA_HOME",
            "JAVA_OPTS",
            "GRADLE_OPTS"
        };

        var maxlen = variables.Max(v => v.Length);

        foreach (var variable in variables.OrderBy(v => v.Length).ThenBy(v => v))
        {
            var padKey = variable.PadLeft(maxlen);
            Information("{0}: {1}", padKey, EnvironmentVariable(variable));
        }

        // java version
        var javaTool = context.Tools.Resolve("java");
                    
        if (javaTool == null)
        {
            javaTool = context.Tools.Resolve("java.exe");
        }

        if (javaTool == null)
        {
            context.Warning("Java not found in path!");
            return;
        }

        Information("Java found at: {0}", javaTool);
        IEnumerable<string> redirectedStandardOutput;
        IEnumerable<string> redirectedStandardError;
        var exitCode = context.StartProcess(
            javaTool,
            new ProcessSettings {
                Arguments = "-version",
                RedirectStandardOutput = true,
                RedirectStandardError = true
            },
            out redirectedStandardOutput,
            out redirectedStandardError
        );

        if (exitCode != 0)
        {
            Warning("Error calling 'java -version'");
            return;
        }
        
        foreach(var l in redirectedStandardOutput.Union(redirectedStandardError))
        {
            Information(l);
        }
    });
