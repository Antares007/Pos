using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using POS.Messages.Application;
using POS.Utils;

namespace POS
{
    [Export, PartCreationPolicy(CreationPolicy.Shared)]
    public class ShellViewModel : Conductor<IScreen>
    {
        private readonly Stack<IScreen> _stack = new Stack<IScreen>();
        //public string BackgroundResource { get; set; }
        private bool _isBusy;
        private Visibility _isKeyboardVisible;

        public bool IsBusy
        {
            get { return _isBusy; }
            set { _isBusy = value; NotifyOfPropertyChange(() => IsBusy); }
        }
        public Visibility IsKeyboardVisible
        {
            get { return _isKeyboardVisible; }
            set { _isKeyboardVisible = value; NotifyOfPropertyChange(() => IsKeyboardVisible); }
        }
        public Visibility IsToolsVisible { get { return ConfigurationManager.AppSettings["pos"] == "pos1" ? Visibility.Collapsed : Visibility.Visible; } }
        //public async void ChangeBackground()
        //{
        //    while (true)
        //    {
        //        BackgroundResource = "";
        //        NotifyOfPropertyChange(() => BackgroundResource);
        //        await Task.Delay(1000);
        //    }
        //}
        [ImportingConstructor]
        public ShellViewModel(IMessageAggregator msgAgr)
        {
            msgAgr.GetStream<ActivateBusyIndicatorRequest>()
                 .Subscribe(req => IsBusy = true);
            msgAgr.GetStream<DeActivateBusyIndicatorRequest>()
                .Subscribe(req => IsBusy = false);
            msgAgr.GetStream<NavigatorActivateScreenRequest>()
                .Subscribe(req => Show(req.Screen));
            msgAgr.GetStream<NavigatorGoBackRequest>()
                .Subscribe(req => GoBack());
            //    ChangeBackground(); 
            IsKeyboardVisible = Visibility.Collapsed;
        }

        public void Show(IScreen screen)
        {
            if (ActiveItem != null)
                _stack.Push(ActiveItem);
            this.ActivateItem(screen);
            NotifyOfPropertyChange(() => ActiveItem);
            NotifyOfPropertyChange(() => CanGoBack);
        }
        public bool CanGoBack { get { return _stack.Count != 0; } }
        public void GoBack()
        {
            var pop = _stack.Pop();
            if (pop != null)
            {
                this.ActivateItem(pop);
                NotifyOfPropertyChange(() => ActiveItem);
                NotifyOfPropertyChange(() => CanGoBack);
            }

        }
        public void ShowKeyboard()
        {
            IsKeyboardVisible = IsKeyboardVisible == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}