using UnityEngine;

namespace Infrastructure.Pool
{
    public interface IObjectPool<T> where T : Component
    {
        T Get(Vector3 position);
        void Return(T item);
    }
}
