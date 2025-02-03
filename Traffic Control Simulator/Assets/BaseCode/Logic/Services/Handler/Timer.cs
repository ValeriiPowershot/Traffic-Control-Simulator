using BaseCode.Interfaces;
using UnityEngine;

namespace BaseCode.Logic.Services.Handler
{
    public abstract class TimerBase : ITimeUsable
    {
        public float Timer { get; set; }
        
        public void AddDelay(float delay)
        {
            Timer = Time.time + delay;
        }

        public bool IsTimerUp()
        {
            return Time.time >= Timer;
        }
    }
}