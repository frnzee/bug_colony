using UnityEngine;

namespace Colony
{
    [CreateAssetMenu(fileName = "ColonyConfig", menuName = "BugColony/Colony Config")]
    public class ColonyConfig : ScriptableObject
    {
        [field: SerializeField] public float SpawnAreaRadius { get; private set; } = 10f;
        [field: SerializeField] public Vector3 SpawnCenter { get; private set; } = Vector3.zero;
    }
}
