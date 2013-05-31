using System;

namespace POS.Utils
{
    public interface IMessageAggregator
    {
        IObservable<T> GetStream<T>();
        void Publish<T>(T payload);
    }
}