using Caliburn.Micro;
using POS.ServerApi;

namespace POS.ViewModels
{
    [Title("suggestions")]
    public class SuggestionsViewModel : HyperMediaViewModel
    {
        public BindableCollection<SuggestionViewModel> Items { get; set; }

        public SuggestionsViewModel()
        {
            Items = new BindableCollection<SuggestionViewModel>();
        }
        public override void UpdateUi(ScreenActivationContext sac)
        {
           Items.Clear();
           Items.AddRange(sac.Jq.All("suggestions", 
               cqq => new SuggestionViewModel()
               {
                   Name = cqq.GetText("suggest"),
                   Link = cqq.GetLink("suggest")
               }));
        }
    }
    public class SuggestionViewModel : Screen
    {
        public string Name { get; set; }
        public TLink Link { get; set; }
    }
}