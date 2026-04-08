using Core;

namespace Bugs.Interaction
{
    public class InteractionService : IInteractionService
    {
        public bool CanInteract(IBug attacker, IEatable target)
        {
            if (target is IBug targetBug)
                return attacker.Type == BugType.Predator && targetBug.Type == BugType.Worker;
            return true;
        }

        public void Interact(IEatable target) => target.BeEaten();
    }
}
