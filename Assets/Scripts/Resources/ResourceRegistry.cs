using System.Collections.Generic;

namespace Resources
{
    public class ResourceRegistry : IResourceRegistry
    {
        private readonly List<IResource> _activeResources = new();

        public IReadOnlyList<IResource> ActiveResources => _activeResources;

        public void Register(IResource resource)
        {
            if (!_activeResources.Contains(resource))
            {
                _activeResources.Add(resource);
            }
        }

        public void Unregister(IResource resource)
        {
            _activeResources.Remove(resource);
        }
    }
}
