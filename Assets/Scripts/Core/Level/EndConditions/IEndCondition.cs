using System;

namespace Variecs.ProjectDII.Core.Level
{
    public interface IEndCondition: IDisposable
    {
        public bool Check();
    }
}