using Core;

namespace Bugs.Interaction
{
    public interface IInteractionService
    {
        bool CanInteract(IBug attacker, IEatable target);
        void Interact(IEatable target);
    }
}
