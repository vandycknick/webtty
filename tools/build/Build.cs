using System;
using Nuke.Common;
using Nuke.Common.Execution;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using System.Runtime.InteropServices;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(build => build.Default);

    [PathExecutable("yarn")]
    readonly Tool Yarn;

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "test";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath BuildOutputDirectory => RootDirectory / ".build";

    AbsolutePath UIDirectory => SourceDirectory / "WebTty.UI";
    AbsolutePath NativeDirectory => SourceDirectory / "WebTty.Native/WebTty.Native.csproj";

    readonly string CliToolName = "webtty";

    Target Default => _ => _
        .Executes(() => Configuration = Configuration.Release)
        .Triggers(Setup)
        .Triggers(Package);

    Target Install => _ => _
        .Executes(() =>
        {
            var buildNumber = GitVersion.RevListHeadCount();

            DotNetToolInstall(s => s
                .AddSources(ArtifactsDirectory)
                .SetGlobal(true)
                .SetVersion($"0.1.0-build.{buildNumber}")
                .SetPackageName(CliToolName));
        });

    Target UnInstall => _ => _
        .Executes(() =>
        {
            DotNetToolUninstall(s => s
                .SetGlobal(true)
                .SetPackageName(CliToolName));
        });

    Target Setup => _ => _
        .Executes(() =>
        {
            Yarn($"install", workingDirectory: UIDirectory);
        })
        .Triggers(Restore);

    Target Clean => _ => _
        .Executes(() =>
        {
            Yarn($"run clean", workingDirectory: UIDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Purge => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            DeleteDirectory(BuildOutputDirectory / "bin");
            DeleteDirectory(BuildOutputDirectory / "obj");
            DeleteDirectory(ArtifactsDirectory);
            DeleteDirectory(UIDirectory / "node_modules");
        });

    Target Test => _ => _
        .Executes(() =>
        {
            if (IsLocalBuild)
            {
                DotNetTest(s => s
                    .SetProjectFile(Solution.GetProject("WebTty.Test")));

                DotNetTest(s => s
                    .SetProjectFile(Solution.GetProject("WebTty.Integration.Test")));
            }
            else
            {
                DotNetTest(s => s
                    .SetProjectFile(Solution.GetProject("WebTty.Test"))
                    .SetNoBuild(true)
                    .SetResultsDirectory(ArtifactsDirectory / "TestResults")
                    .SetLogger("trx"));

                // Temporary remove until I have a better plan to run integration tests in ci
                // DotNetTest(s => s
                //     .SetProjectFile(Solution.GetProject("WebTty.Integration.Test"))
                //     .SetNoBuild(true)
                //     .SetResultsDirectory(ArtifactsDirectory / "TestResults")
                //     .SetLogger("trx"));
            }
        });

    Target Watch => _ => _
        .Executes(() =>
        {
            var ext = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "ps1" : "sh";
            Parallel.Run(
                $"./build.{ext} {nameof(WatchUI)} --no-logo ",
                $"./build.{ext} {nameof(WatchServer)} --no-logo"
            );
        });

    Target WatchUI => _ => _
        .Executes(() =>
        {
            Yarn("run watch", workingDirectory: UIDirectory);
        });

    Target WatchServer => _ => _
        .DependsOn(CompileUI)
        .Executes(() =>
        {
            DotNet("watch run", workingDirectory: SourceDirectory / "WebTty");
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore, CompileUI)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore());
        });

    Target CompileUI => _ => _
        .Executes(() =>
        {
            Yarn($"run build", workingDirectory: UIDirectory);
        });

    Target Package => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .DependsOn(CompileUI)
        .DependsOn(PackageNative)
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetForceEvaluate(true));

            var buildNumber = GitVersion.RevListHeadCount();
            var sourceRevisionId = GitVersion.RevParseHead();

            DotNetBuild(s => s
                .SetProjectFile(Solution.GetProject("WebTty"))
                .SetConfiguration(Configuration)
                .SetProperty("BuildNumber", buildNumber)
                .SetProperty("SourceRevisionId", sourceRevisionId)
                .SetProperty("IsPackaging", true));

            DotNetPack(s => s
                .SetProject(Solution.GetProject("WebTty"))
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetNoBuild(true)
                .SetProperty("BuildNumber", buildNumber)
                .SetProperty("SourceRevisionId", sourceRevisionId)
                .SetProperty("IsPackaging", true));
        });

    Target PackageNative => _ => _
        .DependsOn(Clean)
        .DependsOn(Restore)
        .Executes(() =>
        {
            var suffix = $"build.{DateTime.Now.ToString("yyyyMMddHHmmss")}";

            DotNetPack(s => s
                .SetProject(NativeDirectory)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVersionSuffix(suffix));
        });

}
