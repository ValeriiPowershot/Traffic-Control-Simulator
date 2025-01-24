namespace BaseCode.Interfaces
{
    public interface ITimeUsable
    { 
        public float Timer { get; set; }

        public bool IsTimerUp();

        public void AddDelay(float delay);
    }
}