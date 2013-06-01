using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Windows;
using Caliburn.Micro;
using POS.Messages.Application;
using POS.ServerApi;
using POS.Utils;
using POS.ViewModels;

namespace POS
{
    public class PosBootstrapper : Bootstrapper<ShellViewModel>
    {

        CompositionContainer _container;
        public static IMessageAggregator _msg;
        private StatefulHttpClient _httpClient;
        protected override void Configure()
        {
            _container = new CompositionContainer(new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x))));
            var batch = new CompositionBatch();
            var statefulHttpClient = new StatefulHttpClient(new HttpClient(), new CookieContainer());
            var messageAggregator = new MessageAggregator();
            _msg = messageAggregator;
            _httpClient = statefulHttpClient;
            batch.AddExportedValue<StatefulHttpClient>(statefulHttpClient);
            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IMessageAggregator>(messageAggregator);
            batch.AddExportedValue(_container);
            _container.Compose(batch);
        }

        protected override object GetInstance(Type serviceType, string key)
        {
            var contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
            var exports = _container.GetExportedValues<object>(contract);

            if (exports.Any())
                return exports.First();

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }
        protected async override void OnStartup(object sender, StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            var data = new Dictionary<string, string>() { { "posisNomeri", ConfigurationManager.AppSettings["pos"] } };
            var viewModel = await _httpClient.GetAsync(ConfigurationManager.AppSettings["host"])
                                    .SubmitFormAsync("airchiePosi", data)
                                    .GetScreenAsync();
            _msg.Publish(new NavigatorActivateScreenRequest(viewModel));
            //_msg.Publish(new NavigatorActivateScreenRequest(new MainViewModel()));
        }
    }
}