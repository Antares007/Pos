using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace POS.Utils
{
    public class PosTextBox : TextBox
    {
        private readonly DispatcherTimer _timer;

        public PosTextBox()
        {
            _timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (sender, args) =>
                {
                    _timer.Stop();
                    RaiseExecuteCommandEvent();
                };
        }
        public static readonly RoutedEvent ExecuteCommandEvent = EventManager.RegisterRoutedEvent(
                "ExecuteCommand", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(PosTextBox));
        public event RoutedEventHandler ExecuteCommand
        {
            add { AddHandler(ExecuteCommandEvent, value); }
            remove { RemoveHandler(ExecuteCommandEvent, value); }
        }
        void RaiseExecuteCommandEvent()
        {
            var newEventArgs = new RoutedEventArgs(PosTextBox.ExecuteCommandEvent);
            RaiseEvent(newEventArgs);
        }
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            _timer.Start();
        }
    }
}