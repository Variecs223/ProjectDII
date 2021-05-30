using System;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core
{
    public interface IModel: IDisposable
    {
        InjectorContext ModelType { get; }
    }
}