using Caliburn.Micro;
using POS.ServerApi;

namespace POS.ViewModels
{
    public class SuggestionsViewModel : Screen
    {
        public BindableCollection<SuggestionViewModel> Items { get; set; } 
    }

    public class SuggestionViewModel:Screen
    {
        public string Name { get; set; }
        public TLink Link { get; set; }
    }
}