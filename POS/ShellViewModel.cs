using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Diagnostics;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using Microsoft.Expression.Interactivity.Core;
using POS.ServerApi;
using POS.Utils;
using System.Linq;
using Telerik.Windows.Controls;
using KeyConverter = POS.Utils.KeyConverter;

namespace POS
{
    [Export, PartCreationPolicy(CreationPolicy.Shared)]
    public class ShellViewModel : Conductor<IScreen>, IShell
    {
        private Visibility _isKeyboardVisible;
        public Visibility IsKeyboardVisible
        {
            get { return _isKeyboardVisible; }
            set { _isKeyboardVisible = value; NotifyOfPropertyChange(() => IsKeyboardVisible); }
        }
        public Visibility AreToolsVisible
        {
            get
            {
                return ConfigurationManager.AppSettings["pos"] == "1" || 
                    ConfigurationManager.AppSettings["pos"] == "2" 
            ? Visibility.Collapsed : Visibility.Visible; } }
        public INavigator Navigator { get; set; }
        public ShellViewModel()
        {
            IsKeyboardVisible = Visibility.Collapsed;
        }
        public void ShowKeyboard()
        {
            IsKeyboardVisible = IsKeyboardVisible == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
        public void Show(ScreenActivationContext sac)
        {
            //var screenType = sac.GetScreenType();
            if (ActiveItem != null && this.ActiveItem is IUpdatableScreen && ((IUpdatableScreen)this.ActiveItem).CanHandle(sac))
                ((IUpdatableScreen)ActiveItem).UpdateUi(sac);
            else
                this.ActivateItem(sac.GetScreen());
        } 
        public void GoBack()
        {
            Navigator.GoBack();
        }
        public void KeyDown(KeyEventArgs args)
        {
            Scanner.ObservableKeys.OnNext(KeyConverter.GetCharFromKey(args.Key));
        }
    }
    public static class Scanner
    {
        static Scanner()
        {
            ObservableKeys = new Subject<char>();
        }
        public static ISubject<char> ObservableKeys { get; set; }
    }
}