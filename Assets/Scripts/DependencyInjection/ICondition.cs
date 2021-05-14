using System;

namespace Variecs.ProjectDII.DependencyInjection
{
    public interface ICondition: IDisposable
    {
        bool IsFulfilled(object target);
    }
}