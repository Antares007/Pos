using System.Collections.Generic;
using Caliburn.Micro;

namespace POS.Utils
{
    public static class EnumerableExtensions
    {
         public static BindableCollection<T> ToBindableCollection<T>(this IEnumerable<T> collection)
         {
             return new BindableCollection<T>(collection);
         } 
    }
}