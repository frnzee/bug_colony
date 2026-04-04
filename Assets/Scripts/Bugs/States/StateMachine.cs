namespace Bugs.States
{
    public class StateMachine
    {
        private IBugState _currentState;

        public void ChangeState(IBugState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        public void Tick(float deltaTime)
        {
            _currentState?.Tick(deltaTime);
        }
    }
}
