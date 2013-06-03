using Caliburn.Micro;
using POS.ServerApi;

namespace POS.ViewModels
{
    [Title("suggestions")]
    public class SuggestionsViewModel : Screen,IUpdatableScreen
    {
        public BindableCollection<SuggestionViewModel> Items { get; set; }

        public SuggestionsViewModel()
        {
            Items = new BindableCollection<SuggestionViewModel>();
        }
        public void UpdateUi(ScreenActivationContext sac)
        {
           Items.Clear();
           Items.AddRange(sac.Cqq.All("#suggestions .suggest", cqq => new SuggestionViewModel()
               {
                   Name = cqq.GetText(),
                   Link = cqq.GetLink()
               }));
        }
    }
    public class SuggestionViewModel : Screen
    {
        public string Name { get; set; }
        public TLink Link { get; set; }
    }
}