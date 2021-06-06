namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class EnemyModel: BaseObjectModel
    {
        public override bool AllowObject(ObjectType other)
        {
            return other != ObjectType.Box;
        }
    }
}