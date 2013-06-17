using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Caliburn.Micro;
using System.Linq;

namespace POS.ServerApi
{
    public class ScreenActivationContext
    {
        private static readonly Lazy<Dictionary<string, Type>> Screens =
            new Lazy<Dictionary<string, Type>>(() => Assembly.GetExecutingAssembly()
            .GetTypes()
            .Select(t => new { Type = t, Title = t.GetCustomAttributes(typeof(TitleAttribute), true).FirstOrDefault() })
            .Where(x => x.Title != null).ToDictionary(arg => ((TitleAttribute)arg.Title).Name, arg => arg.Type), true);

        public ScreenActivationContext( Jq jq, Action<HttpRequestMessage> action)
        {
            Jq = jq;
        }

        public Jq Jq { get; private set; }

        public Type GetScreenType()
        {
            var screenName = Jq.GetText("title");
            return Screens.Value[screenName];
        }

        public IScreen GetScreen()
        {
            var screen = (IUpdatableScreen)Activator.CreateInstance(GetScreenType());
            screen.UpdateUi(this);
            return screen;
        }
    }
}