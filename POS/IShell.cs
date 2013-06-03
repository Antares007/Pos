using POS.ServerApi;

namespace POS
{
    public interface IShell
    {
        void Show(ScreenActivationContext sac);
    }
}