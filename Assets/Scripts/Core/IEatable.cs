using UnityEngine;

namespace Core
{
    public interface IEatable
    {
        bool IsAlive { get; }
        Vector3 Position { get; }
        void BeEaten();
    }
}
