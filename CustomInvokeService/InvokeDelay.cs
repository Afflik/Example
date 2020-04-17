using System;


namespace Example
{
    public class InvokeDelay : Invoke
    {
        #region ClassLifeCycles

        public InvokeDelay(Action method, float delayTime) : base(method)
        {
            Time = delayTime;
        }

        #endregion
    }
}