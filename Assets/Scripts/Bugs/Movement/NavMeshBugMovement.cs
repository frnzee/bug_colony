using UnityEngine;
using UnityEngine.AI;

namespace Bugs.Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshBugMovement : MonoBehaviour, IBugMovement
    {
        private const float ReachedDistanceThreshold = 0.2f;

        public bool IsReachedDestination => !MovementAgent.pathPending && (!MovementAgent.hasPath || MovementAgent.remainingDistance <= ReachedDistanceThreshold);

        private NavMeshAgent _agent;
        private NavMeshAgent MovementAgent => _agent ??= GetComponent<NavMeshAgent>();
        
        public void SetSpeed(float speed)
        {
            MovementAgent.speed = speed;
        }

        public void SetDestination(Vector3 destination)
        {
            MovementAgent.isStopped = false;
            MovementAgent.SetDestination(destination);
        }

        public void Stop()
        {
            MovementAgent.isStopped = true;
            MovementAgent.ResetPath();
        }
    }
}
