using System.Collections.Generic;

namespace Resources
{
    public interface IResourceRegistry
    {
        IReadOnlyList<IResource> ActiveResources { get; }
        void Register(IResource resource);
        void Unregister(IResource resource);
    }
}
