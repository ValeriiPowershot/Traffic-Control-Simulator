using BaseCode.Core.EventBase;
using BaseCode.Events.Events;
using UnityEngine;

namespace BaseCode.Events.Handlers
{
    public class UserRegisteredEventHandler : IEventHandler<UserRegisteredNewEvent>
    {
        public void Handle(UserRegisteredNewEvent currentNewEvent)
        {
            Debug.Log($"User Registered: {currentNewEvent.UserName}, Email: {currentNewEvent.Email}, Time: {currentNewEvent.CreatedAt}");
        }
    }
}