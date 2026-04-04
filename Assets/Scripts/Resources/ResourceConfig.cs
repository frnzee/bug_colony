using UnityEngine;

namespace Resources
{
    [CreateAssetMenu(fileName = "ResourceConfig", menuName = "BugColony/Resource Config")]
    public class ResourceConfig : ScriptableObject
    {
        [field: SerializeField] public float SpawnInterval { get; private set; } = 3f;
        [field: SerializeField] public int MaxResourcesOnScene { get; private set; } = 20;
        [field: SerializeField] public float SpawnAreaRadius { get; private set; } = 15f;
    }
}
