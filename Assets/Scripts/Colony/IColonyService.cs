using System;
using System.Collections.Generic;
using Core;

namespace Colony
{
    public interface IColonyService
    {
        IReadOnlyList<IBug> AliveBugs { get; }
        int AliveBugCount { get; }
        void RegisterBug(IBug bug);
        void UnregisterBug(IBug bug);
        event Action OnColonyExtinct;
    }
}
