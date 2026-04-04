using Bugs.Core;
using UnityEngine;

namespace Bugs.Predator
{
    [CreateAssetMenu(fileName = "PredatorConfig", menuName = "BugColony/Predator Config")]
    public class PredatorConfig : BugConfig
    {
        [field: SerializeField] public float LifetimeDuration { get; private set; } = 10f;
        [field: SerializeField] public int FeedCountToSplit { get; private set; } = 3;
    }
}
