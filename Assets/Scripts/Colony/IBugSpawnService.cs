using Core;
using UnityEngine;

namespace Colony
{
    public interface IBugSpawnService
    {
        IBug SpawnWorker(Vector3 position);
        IBug SpawnPredator(Vector3 position);
    }
}
