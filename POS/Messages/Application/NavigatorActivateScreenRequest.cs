using Caliburn.Micro;

namespace POS.Messages.Application
{
    public class NavigatorActivateScreenRequest
    {
        public IScreen Screen;

        public NavigatorActivateScreenRequest(IScreen screen)
        {
            this.Screen = screen;
        }
    }
}