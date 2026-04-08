using Core;
using UnityEngine;

namespace Colony
{
    public interface IBugSpawnService
    {
        IBug Spawn(BugType type, Vector3 position);
    }
}
