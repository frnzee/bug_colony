using UnityEngine;
using UnityEngine.AI;

namespace Extensions
{
    public static class VectorExtensions
    {
        private const int MaxAttempts = 30;
        private const float SnapDistance = 1f;

        public static Vector3 SampleNavMesh(this Vector3 position, float maxDistance = 5f)
        {
            return NavMesh.SamplePosition(position, out var hit, maxDistance, NavMesh.AllAreas) ? hit.position : position;
        }

        public static Vector3 GetRandomNavMeshPoint(this Vector3 center, float radius)
        {
            for (var i = 0; i < MaxAttempts; i++)
            {
                var randomOffset = Random.insideUnitSphere * radius;
                var candidate = center + new Vector3(randomOffset.x, 0f, randomOffset.z);
                if (NavMesh.SamplePosition(candidate, out var hit, SnapDistance, NavMesh.AllAreas))
                    return hit.position;
            }

            var fallbackDir = Random.insideUnitSphere;
            var fallbackPoint = center + new Vector3(fallbackDir.x, 0f, fallbackDir.z).normalized * radius;
            
            return fallbackPoint.SampleNavMesh(radius);
        }
    }
}
