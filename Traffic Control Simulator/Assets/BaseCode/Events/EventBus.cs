using System;
using BaseCode.Core.EventBase;

namespace BaseCode.Events
{
    public class EventBus : IEventBus
    {
        private readonly IEventBusSubscriptionManager _subscriptionManager;
        private readonly IServiceProvider _serviceProvider;
        
        public EventBus(IEventBusSubscriptionManager subscriptionManager)
        {
            _subscriptionManager = subscriptionManager;
        }
        
        public void Publish<TEvent>(TEvent currentEvent) where TEvent : NewEvent
        {
            var eventName = _subscriptionManager.GetEventIdentifier<TEvent>();
            var subscriptions = _subscriptionManager.GetHandlersForEvent(eventName);

            foreach (var subscription in subscriptions)
            {
                // without hardcoding it, create instance of handler
                if (Activator.CreateInstance(subscription.HandlerType) is IEventHandler<TEvent> handler)
                {
                    handler.Handle(currentEvent);
                }
            }
        }

        public void Subscribe<TEvent, TEventHandler>() where TEvent : NewEvent where TEventHandler : IEventHandler<TEvent>
        {
            _subscriptionManager.AddSubscription<TEvent, TEventHandler>();
        }

        public void Unsubscribe<TEvent, TEventHandler>() where TEvent : NewEvent where TEventHandler : IEventHandler<TEvent>
        {
            _subscriptionManager.RemoveSubscription<TEvent, TEventHandler>();
        }
    }
}