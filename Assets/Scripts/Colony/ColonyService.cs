using System;
using System.Collections.Generic;
using Core;

namespace Colony
{
    public class ColonyService : IColonyService
    {
        private readonly List<IBug> _aliveBugs = new();

        public IReadOnlyList<IBug> AliveBugs => _aliveBugs;
        public int AliveBugCount => _aliveBugs.Count;

        public event Action OnColonyExtinct;

        public void RegisterBug(IBug bug)
        {
            bug.OnDied -= HandleBugDied;
            bug.OnDied += HandleBugDied;

            if (!_aliveBugs.Contains(bug))
            {
                _aliveBugs.Add(bug);
            }
        }

        public void UnregisterBug(IBug bug)
        {
            _aliveBugs.Remove(bug);
            bug.OnDied -= HandleBugDied;
        }

        private void HandleBugDied(IBug bug)
        {
            UnregisterBug(bug);

            if (_aliveBugs.Count == 0)
            {
                OnColonyExtinct?.Invoke();
            }
        }
    }
}
