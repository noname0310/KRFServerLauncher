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

        private void ServerInstall_Command_Excute()
        {
            MainWindow.notifyManager.ShowInformation("test");
            _InstallConsole_AppendLine("test");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyUpdate(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }

    public class Command : ICommand
    {   
        //private readonly Action<object> _execute;
        private readonly Action _execute;

        //private readonly Func<object, bool> _canExecute;
        private readonly Func<bool> _canExecute;//instead of prev line 

        //public Command(Action<object> execute) 
        public Command(Action execute)//instead of prev line
          : this(execute, null)
        { }

        //public Command(Action<object> execute,
        public Command(Action execute,//instead of prev line 
        Func<bool> canExecute)//added instead of next line 
                              //Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public void Execute(object parameter)
        {
            //_execute(parameter);
            _execute();//added instead of prev line 
        }
        public bool CanExecute(object parameter)
        {
            return (_canExecute == null)
            //|| _canExecute(parameter);
            || _canExecute();//added instead of prev line 
        }
        public event EventHandler CanExecuteChanged = delegate { };
        public void RaiseCanExecuteChanged()
        { CanExecuteChanged(this, new EventArgs()); }
    }
}
