using System.Windows;
using System.Windows.Media;

namespace POS.Utils
{
    public static class FrameworkElementExtensions
    {
        public static FrameworkElement Up<T>(this FrameworkElement fe, string name)
        {
            var searchType = typeof(T);
            var parent = VisualTreeHelper.GetParent(fe) as FrameworkElement;
            if (parent == null)
                return null;
            if (parent.GetType() == searchType)
            {
                var result = parent.Name == name;
                if (result)
                    return parent;
                else
                    return parent.Up<T>(name);
            }
            else
            {
                return parent.Up<T>(name);
            }
        }
        public static FrameworkElement Down<T>(this FrameworkElement fe, string name)
        {
            FrameworkElement result = null;
            var searchType = typeof(T);
            var childrenCount = VisualTreeHelper.GetChildrenCount(fe);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(fe, i) as FrameworkElement;
                if (child == null)
                    return null;
                else
                {
                    if (child.GetType() == searchType)
                    {
                        var r = child.Name == name;
                        if (r)
                            result = child;
                        else
                        {
                            result = child.Down<T>(name);
                            if (result != null && result.GetType() == searchType && result.Name == name)
                                break;
                        }
                    }
                    else
                    {
                        result = child.Down<T>(name);
                        if (result != null && result.GetType() == searchType && result.Name == name)
                            break;
                    }

                }
            }
            return result;
        }
    }
}