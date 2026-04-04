using System;

namespace Core
{
    public interface IBug : IEatable
    {
        BugType Type { get; }
        event Action<IBug> OnDied;
    }
}
