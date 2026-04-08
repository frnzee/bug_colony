using Core;

namespace Bugs.Core
{
    public interface IBugBehavior
    {
        BugType BugType { get; }
        void OnSessionStart();
        void OnSessionEnd();
        void OnKilled();
    }
}
