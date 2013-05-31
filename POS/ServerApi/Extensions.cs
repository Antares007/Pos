using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Caliburn.Micro;
using CsQuery;
using POS.Messages.Application;
using POS.ViewModels;
using POS.Utils;

namespace POS.ServerApi
{
    public static class Extensions
    {
        private static readonly Dictionary<string, Func<StatefulHttpClient, CQ, Task<StatefulResponse>, Screen>> Factories =
            new Dictionary<string, Func<StatefulHttpClient, CQ, Task<StatefulResponse>, Screen>>()
                {
                    {"main",(client,cq,res) =>
                        {
                            return new MainViewModel();
                        }
                    },
                    {
                        "praislisti", (client,cq,res) =>
                            {
                                Action<HttpRequestMessage> transitLink = async (req) =>
                                    {
                                        var screen = await client.SendAsync(req).GetScreenAsync();
                                        PosBootstrapper._msg.Publish(new NavigatorActivateScreenRequest(screen));
                                    };


                                var vm = new PriceListViewModel
                                    {
                                        Items = cq["div#pricelisti>ul>li"].Select(
                                            x => new PriceListItemViewModel()
                                                {
                                                    ItemName = CQ.Create(x)[".dasakheleba"].Text(),
                                                    Photo = CQ.Create(x)[".surati"].Attr("src"),
                                                    Skus = string.Join(" / ", CQ.Create(x)[".sku"].Select(e => e.InnerText)),
                                                    Eans = string.Join(" / ", CQ.Create(x)[".ean"].Select(e => e.InnerText)),
                                                    Price = CQ.Create(x)[".fasi"].Text()
                                                }).ToBindableCollection(),
                                        Next = new TLink(cq["div#pricelisti a[rel='next']"].Attr("href"), transitLink),
                                        Prev = new TLink(cq["div#pricelisti a[rel='prev']"].Attr("href"), transitLink),
                                        Search = cq.MakeTForm("dzebna",transitLink)
                                    };
                                return vm;
                            }
                    },
                    {
                        "produktebi",(client, cq, res) =>
                            {
                                 Action<HttpRequestMessage> transitLink = async (req) =>
                                    {
                                        var screen = await client.SendAsync(req).GetScreenAsync();
                                        PosBootstrapper._msg.Publish(new NavigatorActivateScreenRequest(screen));
                                    };
                                var productsViewModel = new ProductsViewModel()
                                    {
                                        Items = (from i in cq["#produktebi .yvela .produkti"]
                                                 let pCq = CQ.Create(i)
                                                  select new ProductItemViewModel()
                                                      {
                                                          Name = pCq[".dasakheleba"].Text(),
                                                          Eans = pCq[".eans"].Text(),
                                                          Photo = pCq[".images .image"].Select(x=> x.Attributes["src"]).FirstOrDefault(),
                                                          Open = new TLink(pCq["a"].Attr("href"), transitLink),
                                                      }).ToBindableCollection(),
                                        Search = cq.MakeTForm("dzebna", transitLink)
                                    };
                                return productsViewModel;
                            }
                    },
                    {
                        "suggestions",(client, cq, res) =>
                            {
                                Action<HttpRequestMessage> transitLink = async (req) =>
                                {
                                    var screen = await client.SendAsync(req).GetScreenAsync();
                                    PosBootstrapper._msg.Publish(new NavigatorActivateScreenRequest(screen));
                                };
                                var suggestions = new SuggestionsViewModel()
                                    {
                                        Items = (
                                                    from i in cq["#suggestions .suggest"]
                                                    select new SuggestionViewModel()
                                                        {
                                                            Name = i.InnerText,
                                                            Link = new TLink(i.Attributes["href"], transitLink),
                                                        }
                                                ).ToBindableCollection()
                                    };
                                return suggestions;
                            }
                    },
                    {
                        "produkti",(client, cq, res) =>
                            {
                                Action<HttpRequestMessage> transitLink = async (req) =>
                                {
                                    var screen = await client.SendAsync(req).GetScreenAsync();
                                    PosBootstrapper._msg.Publish(new NavigatorActivateScreenRequest(screen));
                                };
                                var productViewModel = new ProductViewModel()
                                    {
                                        Id = cq[".partiebi .partia .id"].Text(),
                                        Barcode = cq["a[rel='self']"].Text(),
                                        Name = cq[".dasakheleba"].Text(),
                                        ExcelFileName = cq[".partiebi .partia .shenishvna"].Text(),
                                        Price = cq.MakeTForm("fasisShecvla",async message => await client.SendAsync(message)),
                                        Eans = cq[".eans .ean"].Text(),
                                        References = cq[".eans .ref"].Select(x => new ReferenceViewModel()
                                            {
                                                Barcode = x.InnerText, 
                                                Open = new TLink(x.Attributes["href"], transitLink)
                                            }).ToBindableCollection(),
                                        Photos = cq[".images"].Select(x=>x.Attributes["src"]).ToBindableCollection()
                                    };
                                return productViewModel;
                            }
                    }
                };
        public static async Task<StatefulResponse> SubmitFormAsync(this Task<StatefulResponse> msg, string formId,
                                                                   Dictionary<string, string> data)
        {
            var response = await msg;
            CQ q = await response.Message.Content.ReadAsStringAsync();
            var form = q["form#" + formId].First();
            var methodName = form.Attr("method");
            var httpMethod = methodName == "post" ? HttpMethod.Post : HttpMethod.Get;
            var url = form.Attr("action");
            if (httpMethod == HttpMethod.Get)
            {
                url = url + "?" + string.Join("&", data.Select(kv => string.Format("{0}={1}", kv.Key, kv.Value)));
            }

            var requestUri = new Uri(response.Message.RequestMessage.RequestUri, url);

            var message = new HttpRequestMessage(httpMethod, requestUri);
            if (httpMethod == HttpMethod.Post)
            {
                var nameValueCollection = AddDefaults(data, form);
                var content = new FormUrlEncodedContent(nameValueCollection);
                message.Content = content;
            }

            return await response.Client.SendAsync(message);
        }

        private static IEnumerable<KeyValuePair<string, string>> AddDefaults(Dictionary<string, string> data, CQ form)
        {
            var parsers = new Dictionary
                <string, Func<string, CsQuery.IDomObject, Tuple<string, bool, string[], string>>>()
                {
                    {"text", (name, o) => Tuple.Create(name, false, new string[] {null}, o.Attributes["value"])},
                    {"file", (name, o) => Tuple.Create(name, false, new string[] {null}, o.Attributes["value"])},
                    {"hidden", (name, o) => Tuple.Create(name, false, new string[] {null}, o.Attributes["value"])},
                    {"textarea", (name, o) => Tuple.Create(name, false, new string[] {null}, o.Attributes["value"])},
                    {"submit", (name, o) => Tuple.Create(name, true, new[] {o.Attributes["value"]}, default(string))},
                    {
                        "checkbox",
                        (name, o) =>
                        Tuple.Create(name, false, new[] {o.Attributes["value"]},
                                     o.Attributes["checked"] == null ? null : o.Attributes["value"])
                    },
                    {
                        "radio",
                        (name, o) =>
                        Tuple.Create(name, true, new[] {o.Attributes["value"]},
                                     o.Attributes["checked"] == null ? null : o.Attributes["value"])
                    },
                    {
                        "select", (name, o) =>
                            {
                                var opts = o.Cq().Children("option")
                                            .Select(x_ => new
                                                {
                                                    value = x_.Attributes["value"],
                                                    selected = x_.Attributes["selected"] != null
                                                });
                                var fe = Tuple.Create(name, false, opts.Select(x => x.value).ToArray(),
                                                      opts.First(x => x.selected).value);
                                return fe;
                            }
                    },
                };

            var fields = form.Contents()["[name]"]
                             .Select(
                                 x => parsers[(x.Attributes["type"] ?? x.NodeName).ToLower()](x.Attributes["name"], x))
                             .Select(
                                 x => new { Name = x.Item1, OneValue = x.Item2, AllowedValues = x.Item3, Value = x.Item4 }).ToList();

            var options = fields
                .SelectMany(
                    (e, i) =>
                    e.AllowedValues.Select(
                        (av) =>
                        new { e.Name, AllowedValue = av, Group = e.Name + "_" + (e.OneValue ? "0" : i.ToString()) }))
                ;

            var defaults = fields.Where(f => f.Value != null)
                                 .Select(f => new { f.Name, f.Value });

            var values = data.Select(kv => new { Name = kv.Key, Value = kv.Value });

            return (from opt in options
                    from val in values.Concat(defaults).Select((v, i) => new { v, i })
                    where val.v.Name == opt.Name
                    where opt.AllowedValue == null || opt.AllowedValue == val.v.Value
                    group val by opt.Group
                        into g
                        select g.OrderBy(x => x.i).First().v)
                        .Select(x => new KeyValuePair<string, string>(x.Name, x.Value));
        }

        public static async Task<Screen> GetScreenAsync(this Task<StatefulResponse> msg)
        {
            var response = await msg;
            CQ q = await response.Message.Content.ReadAsStringAsync();
            var screenName = q["head title"].First().Text();
            return Factories[screenName](response.Client, q, msg);
        }

        public static TForm MakeTForm(this CQ cq, string formId, Action<HttpRequestMessage> action)
        {
            var data = cq["form#" + formId + " [name]"].Select(x => new
                {
                    Name = x.Attributes["name"],
                    Value = x.Attributes["value"],
                    Required = x.Attributes["required"] != null
                }).ToList();
            var href = cq["form#" + formId].First().Attr("action");
            var m = cq["form#" + formId].First().Attr("method");
            var method = m == "post" ? HttpMethod.Post : HttpMethod.Get;
            return new TForm(method, data.Where(x => x.Required).Select(x => x.Name), href, data.ToDictionary(x => x.Name, x => x.Value), action);
        }
    }
}