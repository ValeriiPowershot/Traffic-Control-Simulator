using BaseCode.Interfaces;
using UnityEngine;

namespace BaseCode.Logic.Services.Handler
{
    public class TimerBase : ITimeUsable
    {
        public float Timer { get; set; } = 0;
        
        public void AddDelay(float delay)
        {
            Timer = Time.time + delay;
        }
        public bool IsTimerUp()
        {
            return Time.time >= Timer;
        }
        public void ResetTimer()
        {
            Timer = Time.time;
        }
    }
}