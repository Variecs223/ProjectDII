namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class BoxModel: BaseObjectModel
    {
        public override bool AllowObject(ObjectType other)
        {
            return false;
        }
    }
}