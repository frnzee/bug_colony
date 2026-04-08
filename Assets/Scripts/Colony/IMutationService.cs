namespace Colony
{
    public interface IMutationService
    {
        bool ShouldMutate(int colonySize);
    }
}
