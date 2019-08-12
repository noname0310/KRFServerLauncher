using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace RSShield_Model.ConsoleProcess
{
    public class ConsoleProcess
    {
        public delegate void ConsoleOutput(string msg);
        public event ConsoleOutput OnConsoleOut;
        public event ConsoleOutput OnConsoleErrorOut;
        
        public event EventHandler OnConsoleExit;

        private Process Console;
        protected string WorkingDirectory = "";
        readonly string WorkingDefaultDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString();

        public ConsoleProcess()
        {
            WorkingDirectory = WorkingDefaultDir;
        }

        public ConsoleProcess(string Directory)
        {
            WorkingDirectory = Directory;
        }

        private void InitProcess()
        {
            ProcessStartInfo StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = "cmd.exe",
                //FileName = "steam\\steamcmd.exe",
                WorkingDirectory = WorkingDirectory,
                WindowStyle = ProcessWindowStyle.Hidden,
                StandardErrorEncoding = Encoding.UTF8,
                StandardOutputEncoding = Encoding.UTF8,
                ErrorDialog = true,
                //ErrorDialogParentHandle = Handle
            };

            Console = new Process
            {
                StartInfo = StartInfo
            }; //변수에 Process 값 대입

            Console.Exited += new EventHandler(ConsoleExit); //서버가 종료되었을때
            //Console.OutputDataReceived += new DataReceivedEventHandler(ConsoleOut); //서버에서 출력메세지가 생겼을때
            //Console.ErrorDataReceived += new DataReceivedEventHandler(ConsoleErrorOut); //서버에서 오류메세지가 생겼을때
        }

        public void StartProcess()
        {
            InitProcess();
            Console.Start(); //서버 시작
            //Console.BeginErrorReadLine(); //에러 메세지 가져오기 시작
            //Console.BeginOutputReadLine(); //서버 출력 메세지 가져오기 시작
            
            Console.StandardInput.AutoFlush = true;

            //ConsoleInput("chcp 65001");
            Task StandardOutHandletask = new Task(new Action(StandardOutread));
            Task StandardErrorHandletask = new Task(new Action(StandardErrorread));
            StandardOutHandletask.Start();
            StandardErrorHandletask.Start();
        }

        private void StandardOutread()
        {
            using (StreamReader StandardOutreader = Console.StandardOutput)
            {
                while (!Console.HasExited)
                {
                    if (!StandardOutreader.EndOfStream)
                    {
                        string procOutput = StandardOutreader.ReadLine();
                        // trying to do the control update directly will result in an invalid cross-thread operation exception
                        // instead, we invoke the control update on the window thread using this.Invoke(...)
                        if (procOutput != null)
                            ConsoleOut(procOutput);
                    }
                    else
                        Thread.Sleep(20);                      // no input, so just wait for 20ms and check again
                }
            }
        }

        private void StandardErrorread()
        {
            using (StreamReader StandardErrorreader = Console.StandardError)
            {
                while (!Console.HasExited)
                {
                    if (!StandardErrorreader.EndOfStream)
                    {
                        string procOutput = StandardErrorreader.ReadLine();
                        // trying to do the control update directly will result in an invalid cross-thread operation exception
                        // instead, we invoke the control update on the window thread using this.Invoke(...)
                        if (procOutput != null)
                            ConsoleErrorOut(procOutput);
                    }
                    else
                        Thread.Sleep(20);                      // no input, so just wait for 20ms and check again
                }
            }
        }

        public void StopProcess()
        {
            Console.Close();
            Console.Dispose();
        }

        private void ConsoleExit(object sender, EventArgs e)
        {
            Console.Close();
            Console.Dispose();
            OnConsoleExit?.Invoke(this, new EventArgs());
        }

        protected virtual void ConsoleOut(string msg) => OnConsoleOutm(msg);

        protected virtual void ConsoleErrorOut(string msg) => OnConsoleErrorOutm(msg);

        protected void OnConsoleOutm(string msg) => OnConsoleOut?.Invoke(msg);

        protected void OnConsoleErrorOutm(string msg) => OnConsoleErrorOut?.Invoke(msg);

        public void ConsoleInput(string CommandLine)
        {
            Console.StandardInput.Write(CommandLine + Environment.NewLine);
        }
    }

    public class InstallProcess : ConsoleProcess
    {
        public delegate void NotifyOutput(InstallConsoleNotify consoleNotify,string msg);
        public event NotifyOutput OnConsoleNotifyOut;
        
        readonly string UpdateScriptDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString()+ "\\CommandScripts\\update_script.txt";
        readonly int InstallProgress;

        public InstallProcess() { }

        public InstallProcess(string Directory)
        {
            WorkingDirectory = Directory;
        }

        protected override void ConsoleOut(string msg)
        {
            OnConsoleOutm(msg);
        }

        public void RustServerUpdate()
        {
            OnConsoleNotifyOut?.Invoke(InstallConsoleNotify.ConsoleStart, null);
            string UpdateCmdLine = @"cd steam
steamcmd.exe +runscript " + UpdateScriptDir + @"
cd ..";
            ConsoleInput(UpdateCmdLine);
        }
    }

    public class ServerProcess : ConsoleProcess
    {
        public ServerProcess() { }

        public ServerProcess(string Directory)
        {
            WorkingDirectory = Directory;
        }

        public void RunServer()
        {
            //OnConsoleNotifyOut?.Invoke(InstallConsoleNotify.ConsoleStart, null);
            string UpdateCmdLine = @"Run.bat";
            ConsoleInput(UpdateCmdLine);
        }
    }

    public enum InstallConsoleNotify
    {
        ConsoleStart,
        ConsoleStop,
        Info,
        Sucess,
        Warn,
        Error
    }
}
