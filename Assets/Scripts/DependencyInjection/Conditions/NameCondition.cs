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
            var injectListAttribute = fieldInfo.GetCustomAttribute<InjectListAttribute>();

            return injectAttribute != null && Name.Equals(injectAttribute.Name) 
                   || injectListAttribute != null && Name.Equals(injectListAttribute.Name);
        }

        public void Dispose()
        {
            Name = null;
            ObjectPool<NameCondition>.Put(this);
        }
    }
}