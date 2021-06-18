namespace Variecs.ProjectDII.Core.Level
{
    public interface IEndConditionChecker
    { 
        bool CheckVictory(EndConditionType type);
        bool CheckDefeat(EndConditionType type);
    }
}