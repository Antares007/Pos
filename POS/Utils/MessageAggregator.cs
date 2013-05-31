using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace POS.Utils
{
   // [Export,PartCreationPolicy(CreationPolicy.Shared)]
    public class MessageAggregator : IMessageAggregator
    {

        private readonly object _observablesByTypeKeyLock = new object();

        private readonly Dictionary<string, Tuple<object, object>> _observablesByTypeKey = new Dictionary<string, Tuple<object, object>>();

        public IObservable<T> GetStream<T>()
        {
            IObservable<T> stream;
            var key = typeof(T).ToString();

            lock (_observablesByTypeKeyLock)
            {
                if (_observablesByTypeKey.ContainsKey(key))
                {
                    Tuple<object, object> tuple = _observablesByTypeKey[key];
                    stream = (IObservable<T>)tuple.Item2;
                }
                else
                {
                    Type specificSubjectType = typeof(Subject<>).MakeGenericType(new[] { typeof(T) });
                    var subject = (Subject<T>)Activator.CreateInstance(specificSubjectType, new object[] { });
                    stream = subject.Publish().RefCount();
                    var tuple = new Tuple<object, object>(subject, stream);
                    _observablesByTypeKey.Add(key, tuple);
                }
            }
            return stream;
        }

        public void Publish<T>(T payload)
        {
            var key = typeof(T).ToString();

            Tuple<object, object> tuple;

            lock (_observablesByTypeKeyLock)
                _observablesByTypeKey.TryGetValue(key, out tuple);

            if (tuple != null)
            {
                ((Subject<T>)tuple.Item1).OnNext(payload);
            }
        }
    }
}