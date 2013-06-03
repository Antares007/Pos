using Caliburn.Micro;
using POS.ServerApi;

namespace POS
{
    public interface IUpdatableScreen:IScreen
    {
        void UpdateUi(ScreenActivationContext sac);
    }
}