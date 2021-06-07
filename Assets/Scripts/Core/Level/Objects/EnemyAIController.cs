using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class EnemyAIController: IController, IInjectable
    {
        [field: Inject] public EnemyData Data { get; protected set; }
        [field: Inject] public BaseObjectModel Model { get; protected set; }
        [field: Inject] public IMovable Movable { get; protected set; }

        [Inject(Name = LevelData.CurrentLevelTag)]
        private LevelController levelController;

        private bool stationary = true;
        
        public void OnInjected()
        {
            Movable.OnMoveEnded += TryStartMove;
            Model.OnRemoved += Dispose;
            
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
        
        public void Update(float deltaTime)
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
            levelController?.RemoveController(this);

            if (Model != null)
            {
                Model.OnRemoved -= Dispose;

                if (Model.Data != null)
                {
                    Model.Data.UnmarkAsInjected(this);
                    Model.Data.UnbindObject(this);
                }
            }

            if (Movable != null)
            {
                Movable.OnMoveEnded -= TryStartMove;
            }
            
            Data = null;
            Model = null;
            Movable = null;
            levelController = null;
        }
    }
}