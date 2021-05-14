using System;

namespace Variecs.ProjectDII.DependencyInjection
{
    public interface IFactory<out TInstance>: IDisposable
    {
        bool ManuallyInjected { get; }
        TInstance GetInstance();
    }
    
    public interface IFactory<out TInstance, in TArgument>: IDisposable
    {
        bool ManuallyInjected { get; }
        TInstance GetInstance(TArgument argument = default);
    }
}