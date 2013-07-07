using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;
using POS.Views.Sale;

namespace POS.Utils
{
    public class SizeSyncBehavior : Behavior<UniformGrid>
    {
        private UniformGrid _grid;
        private int _childrenCount;
        protected override void OnAttached()
        {
            base.OnAttached();
            if (this.AssociatedObject != null)
            {
                this._grid = this.AssociatedObject;
                _grid.Loaded += _grid_Loaded;
                _grid.SizeChanged += _grid_SizeChanged;
                _grid.LayoutUpdated += _grid_LayoutUpdated;
            }
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            _grid.Loaded -= _grid_Loaded;
            _grid.SizeChanged -= _grid_SizeChanged;
            _grid.LayoutUpdated -= _grid_LayoutUpdated;
        }
        void _grid_LayoutUpdated(object sender, EventArgs e)
        {
            if (_grid.Children.Count != 0 && _childrenCount != _grid.Children.Count)
                SetTargetSize();
            _childrenCount = _grid.Children.Count;
        }

        void _grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetTargetSize();
        }

        void _grid_Loaded(object sender, RoutedEventArgs e)
        {
            _childrenCount = _grid.Children.Count;
            SetTargetSize();
        }

        private void SetTargetSize()
        {
            if (_grid.Children.Count != 0)
            {
                var width = ((FrameworkElement)_grid.Children[0]).ActualWidth;
                var height = ((FrameworkElement)_grid.Children[0]).ActualHeight;
                var target = _grid.Up<SaleView>("SaleViewUc").Down<IncreaseDecreaseTool>("IncrDecrTool");
                if (target != null)
                {
                    target.Width = width - 6.5;
                    target.Height = height - 6.5;
                }
            }
        }
    }
}