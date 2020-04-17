using System;


namespace Example
{
    public class InvokeTime : Invoke
    {
        #region Fields

        public float StartTime;
        public float Interval;
        public float Timer;

        #endregion


        #region ClassLifeCycles

        public InvokeTime(Action method, float startTime, float time, float interval) : base(method)
        {
            StartTime = startTime;
            Interval = interval;
            Time = time;
        }

        #endregion
    }
}