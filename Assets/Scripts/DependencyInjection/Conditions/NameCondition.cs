using System.Reflection;

namespace Variecs.ProjectDII.DependencyInjection.Conditions
{
    public class NameCondition: ICondition
    {
        public string Name { get; private set; }

        public NameCondition Update(string name)
        {
            Name = name;
            return this;
        }
        
        public bool IsFulfilled(object target, FieldInfo fieldInfo)
        {
            var injectAttribute = fieldInfo.GetCustomAttribute<InjectAttribute>();

            return Name.Equals(injectAttribute.Name);
        }

        public void Dispose()
        {
            ObjectPool<NameCondition>.Put(this);
        }
    }
}