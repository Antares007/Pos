using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.VisualStyles;
using System.Windows.Interactivity;
using System.Xaml;
using POS.ViewModels.Sale;
using POS.Views.Sale;

namespace POS.Utils
{
    public class SizeDependentItemsBehavior : Behavior<WrapPanel>
    {
        public int ItemsInRow { get; set; }
        public int ItemsInColumn { get; set; }
        private int _childrenCount = 0;
        WrapPanel _target;
        protected override void OnAttached()
        {
            base.OnAttached();
            if (this.AssociatedObject == null)
                return;
            _target = this.AssociatedObject;
            _target.SizeChanged += onSizeChanged;
            _target.Loaded += onLoad;
            _target.LayoutUpdated += OnLayoutUpdated;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            _target.SizeChanged -= onSizeChanged;
            _target.Loaded -= onLoad;
            _target.LayoutUpdated -= OnLayoutUpdated;
        }
        void OnLayoutUpdated(object sender, EventArgs e)
        {
            if (_target.Children.Count != _childrenCount)
                SetChildrenSizes();
            _childrenCount = _target.Children.Count;
        }
        private void onLoad(object sender, EventArgs e)
        {
            SetChildrenSizes();
        }

        private void onSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_target.IsLoaded)
                SetChildrenSizes();
        }

        private void SetChildrenSizes()
        {
            var width = GetChildWidth();
            var height = GetChildHeight();
            foreach (FrameworkElement fe in _target.Children)
            {
                fe.Width = width;
                fe.Height = height;
            }
            var tool = _target.Up<SaleView>("SaleViewUc").Down<IncreaseDecreaseTool>("IncrDecrTool");
            tool.Width = width - 6.5;
            tool.Height = height - 6.5;
        }

        private double GetChildHorizontalMargins()
        {
            if (_target.Children.Count == 0)
                return 8;
            var fe = _target.Children[0] as FrameworkElement;
            return fe.Margin.Left + fe.Margin.Right;
        }
        private double GetChildVerticalMargins()
        {
            if (_target.Children.Count == 0)
                return 8;
            var fe = _target.Children[0] as FrameworkElement;
            return fe.Margin.Top + fe.Margin.Bottom;
        }

        private double GetChildWidth()
        {
            return (_target.ActualWidth / ItemsInRow) - GetChildHorizontalMargins();
        }
        private double GetChildHeight()
        {
            return (_target.ActualHeight / ItemsInColumn) - GetChildVerticalMargins();
        }
    }
}