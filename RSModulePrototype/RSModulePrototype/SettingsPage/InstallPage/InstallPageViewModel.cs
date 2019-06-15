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
    public class InstallPageViewModel : INotifyPropertyChanged
    {
        #region ViewCTRLS
        private ICommand ServerInstall_Command;
        private StringBuilder InstallConsole;

        public ICommand SInstallCommand
        {
            get
            {
                if (ServerInstall_Command == null)
                {
                    ServerInstall_Command = new Command(ServerInstall_Command_Excute);
                }

                return ServerInstall_Command;
            }
            set
            {
                ServerInstall_Command = value;
            }
        }

        public string _InstallConsole
        {
            get
            {
                if (InstallConsole == null)
                {
                    InstallConsole = new StringBuilder();
                }
                return InstallConsole.ToString();
            }
            set
            {
                InstallConsole = new StringBuilder(value);
                OnPropertyUpdate("_InstallConsole");
            }
        }

        private void _InstallConsole_Append(string text)
        {
            InstallConsole.Append(text);
            OnPropertyUpdate("_InstallConsole");
        }

        private void _InstallConsole_AppendLine(string text)
        {
            InstallConsole.AppendLine(text);
            OnPropertyUpdate("_InstallConsole");
        }
        
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyUpdate(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
        #endregion

        private void ServerInstall_Command_Excute()
        {
            InstallProcess installProcess = new InstallProcess();
            installProcess.OnConsoleNotifyOut += InstallProcess_OnConsoleNotifyOut;
            installProcess.OnConsoleOut += InstallProcess_OnConsoleOut;
            installProcess.StartProcess();
            installProcess.RustServerUpdate();
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
            _InstallConsole_AppendLine(msg);
        }
    }
}
