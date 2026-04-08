using UnityEngine;

namespace Bugs.Movement
{
    public interface IBugMovement
    {
        bool IsReachedDestination { get; }
        void SetDestination(Vector3 destination);
        void Stop();
        void SetSpeed(float speed);
    }
}
