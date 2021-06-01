using System;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Objects;

namespace Variecs.ProjectDII.Core.Level
{
    public interface IObjectPackage: IDisposable
    {
        void GetModels(Action<BaseObjectModel> modelAddition);
        void GetControllers(Action<IController> controllerAddition);
        void GetViews(Action<GameObject> viewAddition, Transform parent = null);
    }
}