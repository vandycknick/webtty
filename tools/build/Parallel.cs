using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public class Parallel
{
    public static void Run(params string[] tasks)
    {
        var taskList = new List<Task>();

        foreach (var task in tasks)
        {
            var runner = Task.Run(() => StartProcess(task));
            taskList.Add(runner);
        }

        Task.WaitAny(taskList.ToArray());
    }

    private static async Task StartProcess(string path)
    {
        var procInfo = new ProcessStartInfo
        {
            FileName = path.Split(' ').FirstOrDefault(),
            Arguments = string.Join(' ', path.Split(' ').Skip(1)),
            RedirectStandardOutput = true,
        };

        var process = Process.Start(procInfo);

        while (!process.StandardOutput.EndOfStream)
        {
            var line = await process.StandardOutput.ReadLineAsync();
            Console.WriteLine(line);
        }

        process.Kill();
        process.WaitForExit();
    }
}
