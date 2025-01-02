using System;

namespace BaseCode.Core.EventBase
{
    public abstract class NewEvent
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    
}