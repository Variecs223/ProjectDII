using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class EnemyAIController: IController, IInjectable
    {
        [field: Inject] public EnemyData Data { get; protected set; }
        [field: Inject] public BaseObjectModel Model { get; protected set; }
        [field: Inject] public IMovable Movable { get; protected set; }
        [Inject] private LevelModel levelModel;

        private bool stationary = true;
        
        public void OnInjected()
        {
            Movable.OnMoveEnded += TryStartMove;
            
            stationary = true;
        }

        public void TryStartMove()
        {
            var i = 0;

            while (i < Data.directionPreferences.Length && !Movable.TryStartMove(
                GetAbsoluteDirection(Model.direction, Data.directionPreferences[i]), Data.baseSpeed))
            {
                i++;
            }

            stationary = i >= Data.directionPreferences.Length;
        }
        
        public void Update()
        {
            if (stationary)
            {
                TryStartMove();
            }
        }

        public Direction GetAbsoluteDirection(Direction currentDir, Direction relativeDir)
        {
            return (Direction) (((int) currentDir + (int) relativeDir) % 4);
        }

        public void Dispose()
        {
            Movable.OnMoveEnded -= TryStartMove;
            
            Model.Data.UnmarkAsInjected(this);
            Model.Data.UnbindObject(this);
            Data = null;
            Model = null;
            Movable = null;
            levelModel = null;
        }
    }
}