using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using RSShield_Model.ConsoleProcess;

namespace RSModulePrototype
{
    class ConsoleViewModel : INotifyPropertyChanged
    {
        ServerProcess serverProcess;

        public ConsoleViewModel()
        {
            serverProcess = new ServerProcess();
            //serverProcess.OnConsoleNotifyOut += InstallProcess_OnConsoleNotifyOut;
            serverProcess.OnConsoleOut += InstallProcess_OnConsoleOut;
            serverProcess.OnConsoleErrorOut += InstallProcess_OnConsoleOut;
            serverProcess.StartProcess();
        }

        #region ViewCTRLS
        private ICommand ServerRun_Command;
        private StringBuilder ServerConsole;
        private string InputTextbox;
        private ICommand InputEnter_Command;

        public ICommand SRunCommand
        {
            get
            {
                if (ServerRun_Command == null)
                {
                    ServerRun_Command = new Command(ServerRun_Command_Excute);
                }

                return ServerRun_Command;
            }
            set
            {
                ServerRun_Command = value;
            }
        }

        public string _ServerConsole
        {
            get
            {
                if (ServerConsole == null)
                {
                    ServerConsole = new StringBuilder();
                }
                return ServerConsole.ToString();
            }
            set
            {
                ServerConsole = new StringBuilder(value);
                OnPropertyUpdate("_ServerConsole");
            }
        }

        public string _InputTextbox
        {
            get
            {
                if (InputTextbox == null)
                {
                    InputTextbox = "";
                }
                return InputTextbox;
            }
            set
            {
                InputTextbox = value;
                OnPropertyUpdate("_InputTextbox");
            }
        }

        public ICommand _InputEnter_Command
        {
            get
            {
                if (InputEnter_Command == null)
                {
                    InputEnter_Command = new Command(InputEnter_Command_Excute);
                }

                return InputEnter_Command;
            }
            set
            {
                InputEnter_Command = value;
            }
        }

        private void _ServerConsole_Append(string text)
        {
            if (ServerConsole == null)
            {
                ServerConsole = new StringBuilder();
            }
            ServerConsole.Append(text);
            OnPropertyUpdate("_ServerConsole");
        }

        private void _ServerConsole_AppendLine(string text)
        {
            if (ServerConsole == null)
            {
                ServerConsole = new StringBuilder();
            }
            ServerConsole.AppendLine(text);
            OnPropertyUpdate("_ServerConsole");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyUpdate(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
        #endregion

        private void ServerRun_Command_Excute()
        {
            serverProcess.RunServer();
        }
        
        private void InputEnter_Command_Excute()
        {
            var bytes = Encoding.UTF8.GetBytes(_InputTextbox);
            string CmdLine = new string(bytes.Select(b => (char)b).ToArray());
            serverProcess.ConsoleInput(CmdLine);
            _InputTextbox = "";
        }

        private void InstallProcess_OnConsoleNotifyOut(InstallConsoleNotify consoleNotify, string msg)
        {
            switch (consoleNotify)
            {
                case InstallConsoleNotify.ConsoleStart:
                    MainWindow.notifyManager.ShowInformation("서버 업데이트를 시작합니다");
                    break;
                default:
                    break;
            }
        }

        private void InstallProcess_OnConsoleOut(string msg)
        {
            _ServerConsole_AppendLine(msg);
        }
    }
}
