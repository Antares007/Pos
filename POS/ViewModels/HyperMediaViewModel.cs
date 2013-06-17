using Caliburn.Micro;
using POS.ServerApi;
using System.Linq;

namespace POS.ViewModels
{
    public class HyperMediaViewModel:Screen,IUpdatableScreen
    {
        public virtual void UpdateUi(ScreenActivationContext sac)
        {
        }

        public bool CanHandle(ScreenActivationContext sac)
        {
            var attribute = this.GetType().GetAttributes<TitleAttribute>(false).First();
            var result = sac.Jq.IsTitle(attribute.Name);
            return result || DoCanHandle(sac);
        }

        protected virtual bool DoCanHandle(ScreenActivationContext sac)
        {
            return false;
        }
    }
}