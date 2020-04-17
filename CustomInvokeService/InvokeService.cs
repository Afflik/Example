using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Example
{
    public class InvokeService : Service, IInvokable
    {
        #region PrivateData

        private List<InvokeDelay> _invokeMethods = new List<InvokeDelay>();
        private List<InvokeTime> _invokeRepeatingMethods = new List<InvokeTime>();
        private List<InvokeDelay> _idInvokeMethodsForRemove = new List<InvokeDelay>();
        private List<InvokeTime> _idInvokeRepeatingMethodsForRemove = new List<InvokeTime>();

        private float _bias = 0.2f;

        #endregion


        #region ClassLifeCycles

        public InvokeService(Context context) : base(context)
        {
        }

        #endregion


        #region Methods

        public void Update()
        {
            foreach (var invokeDelay in _invokeMethods)
            {
                if (invokeDelay.IsCancelInvoke)
                {
                    _idInvokeMethodsForRemove.Add(invokeDelay);
                }
                else if (Math.Abs(Time.time - invokeDelay.Time) <= _bias)
                {
                    invokeDelay.Method();
                    _idInvokeMethodsForRemove.Add(invokeDelay);
                }
            }

            foreach (var invokeTime in _invokeRepeatingMethods)
            {
                if (invokeTime.IsCancelInvoke)
                {
                    _idInvokeRepeatingMethodsForRemove.Add(invokeTime);
                }
                else if (Math.Abs(invokeTime.Time) < 0.01f)
                {
                    invokeTime.Method();
                    _idInvokeRepeatingMethodsForRemove.Add(invokeTime);
                }
                else if (Time.time - invokeTime.StartTime <= invokeTime.Time)
                {
                    if (Math.Abs(Time.time - invokeTime.Timer) <= _bias)
                    {
                        invokeTime.Timer = Time.time + invokeTime.Interval;
                        invokeTime.Method();
                    }
                }
                else
                {
                    _idInvokeRepeatingMethodsForRemove.Add(invokeTime);
                }
            }
        }

        public void CleanUp()
        {
            foreach (var invokeDelay in _idInvokeMethodsForRemove)
            {
                _invokeMethods.Remove(invokeDelay);
            }

            foreach (var invokeTime in _idInvokeRepeatingMethodsForRemove)
            {
                _invokeRepeatingMethods.Remove(invokeTime);
            }

            _idInvokeMethodsForRemove.Clear();
            _idInvokeRepeatingMethodsForRemove.Clear();
        }

        #endregion


        #region IInvokable

        public void Invoke(Action method, float delayTime)
        {
            var time = Time.time + delayTime + _bias;
            _invokeMethods.Add(new InvokeDelay(method, time));
        }

        public void InvokeRepeating(Action method, float time, float interval, Action callback = null)
        {
            var invokeTime = new InvokeTime(method, Time.time, time, interval);
            invokeTime.Interval += _bias;
            invokeTime.Timer = Time.time;

            _invokeRepeatingMethods.Add(invokeTime);
        }

        public void CancelInvoke(Action method)
        {
            var invoke = _invokeMethods.FirstOrDefault(q => q.Method == method);

            if (invoke != null)
            {
                invoke.IsCancelInvoke = true;
            }

            var invokeTime = _invokeRepeatingMethods.FirstOrDefault(q => q.Method == method);

            if (invokeTime != null)
            {
                invokeTime.IsCancelInvoke = true;
            }
        }

        #endregion
    }
}