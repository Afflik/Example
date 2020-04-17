using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Example
{
    public class PoolObject<T> : IPoolObject<T> where T : IBaseModel
    {
        #region Fields

        public int Count => _pool.Count;

        #endregion

        #region PrivateData

        private readonly Queue<T> _pool;
        private readonly Func<T> _func;

        #endregion


        #region ClassLifeCycles

        public PoolObject()
        {
            _pool = new Queue<T>();
        }

        public PoolObject(Func<T> func)
        {
            if (func == null)
            {
                return;
            }

            _pool = new Queue<T>();
            _func = func;
        }

        #endregion


        #region Methods

        public T GetObject(int id, Func<T> func)
        {
            var result = default(T);
            var itemsMemory = new Queue<T>();

            if (_pool.Any(q => q.Id == id))
            {
                while (_pool.Count > 0)
                {
                    var item = _pool.Dequeue();
                        if (item.Id == id)
                        {
                            result = item;
                            break;
                        }

                        itemsMemory.Enqueue(item);
                }
            }
            else
            {
                result = func();
            }
            PutObjects(itemsMemory);
            return result;
        }

        public void PutObject(T item)
        {
            _pool.Enqueue(item);
        }

        public void PutObjects(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                PutObject(item);
            }
        }

        public T Find(int id)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}