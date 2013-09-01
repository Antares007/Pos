using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interactivity;
using POS.ViewModels.Sale;

namespace POS.Utils
{
    public class ResourceKeyBindingBehavior : Behavior<TextBlock>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += (sender, args) =>
                {
                    var viewModel = AssociatedObject.DataContext as AmountItemViewModel;
                    if (viewModel != null)
                    {
                        AssociatedObject.SetResourceReference(TextBlock.TextProperty,viewModel.StringResourceKey);
                    }
                };
        }
    }
}