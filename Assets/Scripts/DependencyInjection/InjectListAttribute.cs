using System;

namespace Variecs.ProjectDII.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InjectListAttribute: Attribute
    {
        public string Name;
    }
}