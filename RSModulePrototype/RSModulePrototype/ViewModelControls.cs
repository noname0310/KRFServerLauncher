using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RSModulePrototype
{
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

    public class AutoScrollBehavior
    {
        public static readonly DependencyProperty AutoScrollProperty =
            DependencyProperty.RegisterAttached("AutoScroll", typeof(bool), typeof(AutoScrollBehavior), new PropertyMetadata(false, AutoScrollPropertyChanged));


        public static void AutoScrollPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var scrollViewer = obj as ScrollViewer;
            if (scrollViewer != null && (bool)args.NewValue)
            {
                scrollViewer.ScrollChanged += ScrollViewer_ScrollChanged;
                scrollViewer.ScrollToEnd();
            }
            else
            {
                scrollViewer.ScrollChanged -= ScrollViewer_ScrollChanged;
            }
        }

        private static void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // Only scroll to bottom when the extent changed. Otherwise you can't scroll up
            if (e.ExtentHeightChange != 0)
            {
                var scrollViewer = sender as ScrollViewer;
                scrollViewer?.ScrollToBottom();
            }
        }

        public static bool GetAutoScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollProperty);
        }

        public static void SetAutoScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollProperty, value);
        }
    }
}
