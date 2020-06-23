using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace saucer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Process unturnedEXE = new Process())
            {
                unturnedEXE.StartInfo.FileName = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Unturned\\Unturned.exe";
                unturnedEXE.StartInfo.UseShellExecute = false;
                unturnedEXE.StartInfo.CreateNoWindow = true;
                unturnedEXE.StartInfo.RedirectStandardOutput = true;
                unturnedEXE.StartInfo.RedirectStandardInput = true;
                unturnedEXE.StartInfo.RedirectStandardError = true;
                unturnedEXE.StartInfo.Arguments = "-nographics -batchmode -NoRedirectConsoleInput -NoRedirectConsoleOutput +secureserver/saucerserver";
                unturnedEXE.OutputDataReceived += (sender, args) => WriteEvent(args.Data);
                unturnedEXE.ErrorDataReceived += (sender, args) => WriteEvent(args.Data);


                unturnedEXE.Start();
                Console.WriteLine("Starting server, please wait half a second.");
                unturnedEXE.BeginOutputReadLine();
                unturnedEXE.BeginErrorReadLine();

                Thread.Sleep(1000 * 60);
                int secondsToWait = 60 * 2;
                for (int i = 0; i < secondsToWait; i++)
                {
                    Thread.Sleep(1000);
                    string message = String.Format("Waiting to read more {0}/{1}...", i, secondsToWait);
                    Console.WriteLine(message);
                    if (i % 10 == 0)
                    {
                        serverCommand(unturnedEXE, string.Format("say {0}", message));
                    }
                }
                serverCommand(unturnedEXE, "say shutting down in two seconds!\n");
                Thread.Sleep(2000);
                serverCommand(unturnedEXE, "save");
                Console.WriteLine("waiting for save to finish...");
                Thread.Sleep(1000 * 2);
                Console.WriteLine("Exiting...");
                serverCommand(unturnedEXE, "shutdown");
                Thread.Sleep(2000);
                unturnedEXE.WaitForExit();
            }

        }

        private static void serverCommand(Process serverProc, string command)
        {
            Console.WriteLine(String.Format("Telling Server to {0}",command));
            StreamWriter writer = serverProc.StandardInput;
            writer.WriteLine(command);
            writer.Flush();
        }

        private static void WriteEvent(string output)
        {
            Console.WriteLine(output);
        }
    }
}
