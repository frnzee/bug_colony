namespace Bugs.States
{
    public interface IBugState
    {
        void Enter();
        void Tick(float deltaTime);
        void Exit();
    }
}
