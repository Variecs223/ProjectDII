using System;
using UnityEngine;

namespace Variecs.ProjectDII.Core
{
    public interface IModel: IDisposable
    {
        ScriptableObject ModelType { get; }
    }
}