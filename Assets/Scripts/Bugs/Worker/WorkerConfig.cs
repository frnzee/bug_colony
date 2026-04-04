using Bugs.Core;
using UnityEngine;

namespace Bugs.Worker
{
    [CreateAssetMenu(fileName = "WorkerConfig", menuName = "BugColony/Worker Config")]
    public class WorkerConfig : BugConfig
    {
        [field: SerializeField] public int FeedCountToSplit { get; private set; } = 2;
        [field: SerializeField] public float MutationChance { get; private set; } = 0.1f;
        [field: SerializeField] public int ColonySizeForMutation { get; private set; } = 10;
    }
}
