using System;
using System.Collections.Generic;
using System.Linq;
using BaseCode.Core.EventBase;

namespace BaseCode.Events
{
    public class EventBusSubscriptionManager : IEventBusSubscriptionManager
    {
        private readonly Dictionary<string, List<Subscription>> _subscriptions = new();
        public event EventHandler<string> OnEventRemoved;

        public bool IsEmpty => !_subscriptions.Any();

        public bool HasSubscriptionsForEvent(string eventName)
        {
            return _subscriptions.ContainsKey(eventName);
        }

        public string GetEventIdentifier<TEvent>()
        {
            return typeof(TEvent).Name;
        }

        public Type GetEventTypeByName(string eventName)
        {
            return _subscriptions.Keys
                .Select(Type.GetType)
                .FirstOrDefault();
        }

        public IEnumerable<Subscription> GetHandlersForEvent(string eventName)
        {
            return _subscriptions.TryGetValue(eventName, out var handlers) ? handlers : new List<Subscription>();
        }

        public Dictionary<string, List<Subscription>> GetAllSubscriptions()
        {
            return _subscriptions;
        }

        public void AddSubscription<TEvent, TEventHandler>()
            where TEvent : NewEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = GetEventIdentifier<TEvent>();
            if (!_subscriptions.ContainsKey(eventName))
            {
                _subscriptions[eventName] = new List<Subscription>();
            }

            _subscriptions[eventName].Add(new Subscription(typeof(TEvent), typeof(TEventHandler)));
        }

        public void RemoveSubscription<TEvent, TEventHandler>()
            where TEvent : NewEvent
            where TEventHandler : IEventHandler<TEvent>
        {
            var eventName = GetEventIdentifier<TEvent>();
            var subscription = _subscriptions[eventName]
                .FirstOrDefault(s => s.HandlerType == typeof(TEventHandler));

            if (subscription == null) return;
            
            _subscriptions[eventName].Remove(subscription);

            if (_subscriptions[eventName].Any()) return;
            
            _subscriptions.Remove(eventName);
            OnEventRemoved?.Invoke(this, eventName);
        }

        public void Clear()
        {
            _subscriptions.Clear();
        }
         
    }
}