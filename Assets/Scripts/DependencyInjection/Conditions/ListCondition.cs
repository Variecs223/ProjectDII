using System.Linq;
using System.Reflection;

namespace Variecs.ProjectDII.DependencyInjection.Conditions
{
    public class ListCondition: ICondition
    {
        public bool IsFulfilled(object target, FieldInfo fieldInfo)
        {
            return fieldInfo.GetCustomAttributes<InjectListAttribute>().Any();
        }

        public void Dispose()
        {
            ObjectPool<ListCondition>.Put(this);
        }
    }
}