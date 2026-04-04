using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure.Pool
{
    public class ObjectPool<T> : IObjectPool<T> where T : Component
    {
        private readonly Stack<T> _inactive = new();
        private readonly Func<T> _factory;

        public ObjectPool(Func<T> factory) => _factory = factory;

        public T Get(Vector3 position)
        {
            var item = _inactive.Count > 0 ? _inactive.Pop() : _factory();
            item.transform.position = position;
            item.gameObject.SetActive(true);
            return item;
        }

        public void Return(T item)
        {
            _inactive.Push(item);
        }
    }
}
