using System;


namespace Example
{
    public abstract class Invoke
    {

        #region Fields

        public readonly Action Method;
        public bool IsCancelInvoke;
        public float Time;

        #endregion


        #region ClassLifeCycles

        protected Invoke(Action method)
        {
            Method = method;
        }

        #endregion
    }
}