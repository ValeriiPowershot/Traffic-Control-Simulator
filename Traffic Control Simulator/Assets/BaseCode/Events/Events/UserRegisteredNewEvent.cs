using BaseCode.Core.EventBase;

namespace BaseCode.Events.Events
{
    public class UserRegisteredNewEvent : NewEvent
    {
        public string UserName { get; set; }
        public string Email { get; set; }

        public UserRegisteredNewEvent(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }
    }
}