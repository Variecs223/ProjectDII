using System;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public interface IMovable
    {
        bool TryStartMove(Direction direction, float speed);
        bool TryEndMove();

        event Action OnMoveEnded;
    }
}