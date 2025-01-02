namespace BaseCode.Core.EventBase
{
    public interface IEventHandler<in TEvent> where TEvent : NewEvent
    {
        void Handle(TEvent currentEvent);
    }
}