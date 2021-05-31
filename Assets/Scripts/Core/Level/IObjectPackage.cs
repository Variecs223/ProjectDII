using System;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Objects;

namespace Variecs.ProjectDII.Core.Level
{
    public interface IObjectPackage: IDisposable
    {
        void AddModels(Action<BaseObjectModel> modelAddition);
        void AddControllers(Action<IController> controllerAddition);
        void AddViews(Action<GameObject> viewAddition);
    }
}