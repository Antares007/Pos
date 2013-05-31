using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using Caliburn.Micro;
using POS.Messages.Application;
using POS.ServerApi;

namespace POS.ViewModels
{
    public class PriceListViewModel : Screen
    {
        public TLink Next { get; set; }
        public TLink Prev { get; set; }
        public BindableCollection<PriceListItemViewModel> Items { get; set; }
        public TForm Search { get; set; }
    }
}