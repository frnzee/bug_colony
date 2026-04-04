using UnityEngine;

namespace Bugs.Core
{
    [CreateAssetMenu(fileName = "BugConfig", menuName = "BugColony/Bug Config")]
    public class BugConfig : ScriptableObject
    {
        [field: SerializeField] public float MoveSpeed { get; private set; } = 3f;
        [field: SerializeField] public float WanderRadius { get; private set; } = 10f;
        [field: SerializeField] public float DetectionRadius { get; private set; } = 10f;
        [field: SerializeField] public float EatDistance { get; private set; } = 1f;
        [field: SerializeField] public float WanderChangeInterval { get; private set; } = 3f;
    }
}
