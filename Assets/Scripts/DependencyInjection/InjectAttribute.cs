using System;

namespace Variecs.ProjectDII.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectAttribute: Attribute
    {
        public string Name;
        public bool Optional;
    }
}