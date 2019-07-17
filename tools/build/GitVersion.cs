using System.Diagnostics;

public class GitVersion
{
    private const string GitCommand = "git";

    public static string RevListHeadCount()
    {
        var info = new ProcessStartInfo
        {
            FileName = GitCommand,
            Arguments = "rev-list --count HEAD",
            RedirectStandardOutput = true,
        };
        var process = Process.Start(info);

        var result = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new System.Exception("Error executing RevParseHead");
        }

        return result.Trim();
    }

    public static string RevParseHead()
    {
        var info = new ProcessStartInfo
        {
            FileName = GitCommand,
            Arguments = "rev-parse HEAD",
            RedirectStandardOutput = true,
        };
        var process = Process.Start(info);

        var result = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new System.Exception("Error executing RevParseHead");
        }

        return result.Trim();
    }
}
