using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
        
        public ConsoleProcess() { }

        public ConsoleProcess(string Directory)
        {
            WorkingDirectory = Directory;
        }

        private void InitProcess()
        {
            Console = new Process(); //변수에 Process 값 대입
            Console.StartInfo.FileName = "cmd.exe";

            if (WorkingDirectory != "")
                Console.StartInfo.WorkingDirectory = WorkingDirectory;
            else
            Console.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;

            Console.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; //검은창 안뜨게
            Console.EnableRaisingEvents = true; //이벤트가 발생되도록
            Console.StartInfo.UseShellExecute = false;
            Console.StartInfo.RedirectStandardInput = true; //명령어를 입력할수 있도록 할지
            Console.StartInfo.CreateNoWindow = true; //창이 안뜨게
            Console.StartInfo.RedirectStandardOutput = true; //서버 출력 메세지를 받아올지
            Console.StartInfo.RedirectStandardError = true; //에러 출력 메세지를 받아올지

            Console.Exited += new EventHandler(ConsoleExit); //서버가 종료되었을때
            Console.OutputDataReceived += new DataReceivedEventHandler(ConsoleOut); //서버에서 출력메세지가 생겼을때
            Console.ErrorDataReceived += new DataReceivedEventHandler(ConsoleErrorOut); //서버에서 오류메세지가 생겼을때
        }

        public void StartProcess()
        {
            InitProcess();
            Console.Start(); //서버 시작
            Console.BeginErrorReadLine(); //에러 메세지 가져오기 시작
            Console.BeginOutputReadLine(); //서버 출력 메세지 가져오기 시작
            ConsoleInput("chcp 65001");
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

        protected virtual void ConsoleOut(object sender, DataReceivedEventArgs e) => OnConsoleOutm(e.Data);

        protected virtual void ConsoleErrorOut(object sender, DataReceivedEventArgs e) => OnConsoleErrorOutm(e.Data);

        protected void OnConsoleOutm(string msg) => OnConsoleOut?.Invoke(msg);

        protected void OnConsoleErrorOutm(string msg) => OnConsoleErrorOut?.Invoke(msg);

        public void ConsoleInput(string CommandLine)
        {
            Console.StandardInput.WriteLine(CommandLine);
        }
    }

    public class InstallProcess : ConsoleProcess
    {
        public delegate void NotifyOutput(InstallConsoleNotify consoleNotify,string msg);
        public event NotifyOutput OnConsoleNotifyOut;

        readonly string RustInstallDir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).ToString();
        readonly int InstallProgress;

        public InstallProcess() { }

        public InstallProcess(string Directory)
        {
            WorkingDirectory = Directory;
        }

        protected override void ConsoleOut(object sender, DataReceivedEventArgs e)
        {
            OnConsoleOutm(e.Data);
        }

        public void RustServerUpdate()
        {
            OnConsoleNotifyOut?.Invoke(InstallConsoleNotify.ConsoleStart, null);

            string UpdateCmdLine = @"cd steam
steamcmd.exe +@ShutdownOnFailedCommand 1
@NoPromptForPassword 1
login anonymous
force_install_dir
" + RustInstallDir + 
@"app_update 258550 validate
quit
cd ..
exit";
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
