using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ShellCommand
{
    class Program
    {
        private static readonly AutoResetEvent waitHandle = new AutoResetEvent(false);

        static void Main(string[] args)
        {
            Console.Clear();

            Console.WriteLine("Press CTRL+C to interrupt the read operation:");

            Task.Run(() =>
            {
                while (true)
                {
                    Console.WriteLine($"Enter command:");

                    string command = Console.ReadLine();

                    string result = RunBashCommand(command);

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(result);
                    Console.ResetColor();
                }
            });

            Console.CancelKeyPress += (o, e) =>
            {
                Console.WriteLine("Exit");

                waitHandle.Set();
            };

            waitHandle.WaitOne();
        }

        public static string RunBashCommand(string arguments)
        {
            return RunProcess(@"/bin/bash", arguments);
        }

        public static string RunProcess(string fileName, string arguments)
        {
            string escapedArgs = arguments.Replace("\"", "\\\"");

            Process process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();

            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadLine();
            
            process.WaitForExit();

            if (!string.IsNullOrWhiteSpace(error))
            {
                Console.WriteLine(error);
            }

            return result;
        }
    }
}
