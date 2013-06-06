using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace POS.Utils
{
    public class StateBindingBehavior : Behavior<UserControl>
    {
        private INotifyPropertyChanged _dataContext;
        private const string Viewstate = "ViewState";
        private PropertyInfo _stateProperty;

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += (sender, args) =>
                {
                    var dataContext = this.AssociatedObject.DataContext;
                    var stateProperty = dataContext.GetType().GetProperty(Viewstate);
                    if (stateProperty != null && dataContext is INotifyPropertyChanged)
                    {
                        _stateProperty = stateProperty;
                        _dataContext = ((INotifyPropertyChanged)dataContext);
                        _dataContext.PropertyChanged += StateBindingBehavior_PropertyChanged;
                    }
                    var stateName = _stateProperty.GetValue(_dataContext).ToString();
                    VisualStateManager.GoToState(this.AssociatedObject, stateName, true);
                };

        }

        void StateBindingBehavior_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != Viewstate)
                return;
            var stateName = _stateProperty.GetValue(_dataContext).ToString();
            VisualStateManager.GoToState(this.AssociatedObject, stateName, true);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (_dataContext != null)
                _dataContext.PropertyChanged -= StateBindingBehavior_PropertyChanged;
        }
    }
}