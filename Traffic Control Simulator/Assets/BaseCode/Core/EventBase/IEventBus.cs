namespace BaseCode.Core.EventBase
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent @event)
            where TEvent : NewEvent;

        void Subscribe<TEvent, TEventHandler>()
            where TEvent : NewEvent
            where TEventHandler : IEventHandler<TEvent>;

        void Unsubscribe<TEvent, TEventHandler>()
            where TEvent : NewEvent
            where TEventHandler : IEventHandler<TEvent>;
    }
    
}