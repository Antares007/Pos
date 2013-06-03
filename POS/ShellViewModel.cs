using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using CsQuery;
using POS.ServerApi;
using POS.Utils;
using System.Linq;

namespace POS
{
    [Export, PartCreationPolicy(CreationPolicy.Shared)]
    public class ShellViewModel : Conductor<IScreen>,IShell
    {
        private Visibility _isKeyboardVisible;
        public Visibility IsKeyboardVisible
        {
            get { return _isKeyboardVisible; }
            set { _isKeyboardVisible = value; NotifyOfPropertyChange(() => IsKeyboardVisible); }
        }
        public Visibility AreToolsVisible { get { return ConfigurationManager.AppSettings["pos"] == "pos1" ? Visibility.Collapsed : Visibility.Visible; } }

        public INavigator Navigator { get; set; }

        [ImportingConstructor]
        public ShellViewModel(IMessageAggregator msgAgr)
        {
            IsKeyboardVisible = Visibility.Collapsed;
        }

        public void ShowKeyboard()
        {
            IsKeyboardVisible = IsKeyboardVisible == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Show(ScreenActivationContext screenActivationContext)
        {
            var screenType = screenActivationContext.GetScreenType();
            if (ActiveItem != null && this.ActiveItem is IUpdatableScreen && this.ActiveItem.GetType() == screenType)
                ((IUpdatableScreen)ActiveItem).UpdateUi(screenActivationContext);
            else
               this.ActivateItem(screenActivationContext.GetScreen());
        }
        public void GoBack()
        {
            Navigator.GoBack();
        }
    }
}