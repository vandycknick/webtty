using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public class Parallel
{
    public static void Run(params string[] targets)
    {
        var taskList = new List<RunningTask>();

        void OnProcessExit(object sender, EventArgs e)
        {
            foreach (var task in taskList)
            {
                Console.WriteLine($"Shutting down process with id {task.Process.Id}");
                if (!task.Process.HasExited) task.Process.Kill();
            }
        }

        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        Console.CancelKeyPress += OnProcessExit;

        foreach (var target in targets)
        {
            var task = RunningTask.Start(target);
            taskList.Add(task);
        }

        Task.WaitAny(taskList.Select(r => r.Task).ToArray());
    }

    private class RunningTask
    {
        public static RunningTask Start(string path)
        {
            var procInfo = new ProcessStartInfo
            {
                FileName = path.Split(' ').FirstOrDefault(),
                Arguments = string.Join(' ', path.Split(' ').Skip(1)),
                RedirectStandardOutput = true,
            };

            var process = Process.Start(procInfo);
            var redirection = Task.Run(() =>
            {

                while (!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    Console.WriteLine(line);
                }
            });

            return new RunningTask
            {
                Process = process,
                Task = redirection,
            };
        }
        public Process Process { get; set; }
        public Task Task { get; set; }

    }
}
