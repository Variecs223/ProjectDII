using System;
using System.Linq;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.Objects
{
    public class MovableController: IController, IMovable
    {
        [field: Inject] public BaseObjectModel Model { get; protected set; }
        [Inject] private LevelModel levelModel;

        private float excess;

        public event Action OnMoveEnded;
        
        public void Update()
        {
            Model.coords += (Vector2)Dir2Vel(Model.direction) * Model.speed;

            if (TryEndMove())
            {
                OnMoveEnded?.Invoke();
            }
        }

        public bool TryEndMove()
        {
            var enteringTileCoords = new Vector2Int(Mathf.RoundToInt(Model.coords.x), Mathf.RoundToInt(Model.coords.y));
            var leavingTileCoords = enteringTileCoords;
            var enteringTile = levelModel.Tiles[enteringTileCoords.y * levelModel.Data.fieldSize.x + enteringTileCoords.x];
            var leavingTile = enteringTile;

            var objectTransition = enteringTile.objects.FirstOrDefault(transition => transition.Object == Model);

            if (objectTransition.Object == null)
            {
                Debug.LogError($"Haven't found the object {Model} in tile {enteringTileCoords}");
                return false;
            }
            
            if (objectTransition.State == TransitionState.Leaving)
            {
                enteringTileCoords += Dir2Vel(Model.direction);
                enteringTile = levelModel.Tiles[enteringTileCoords.y * levelModel.Data.fieldSize.x + enteringTileCoords.x];
            }
            else
            {
                leavingTileCoords -= Dir2Vel(Model.direction);
                leavingTile = levelModel.Tiles[leavingTileCoords.y * levelModel.Data.fieldSize.x + leavingTileCoords.x];
            }

            var diff = (Model.coords - enteringTileCoords - Vector2.one * 0.5f) * Dir2Vel(Model.direction);

            if (diff.x <= 0 && diff.y <= 0)
            {
                return false;
            }
            
            excess = (Model.coords - enteringTileCoords).magnitude;
            leavingTile.objects.RemoveAll(obj => obj.Object == Model);
            var objIndex = enteringTile.objects.FindIndex(obj => obj.Object == Model);

            if (objIndex == -1)
            {
                Debug.LogError($"Haven't found the object {Model} in tile {enteringTileCoords}");
                return false;
            }

            enteringTile.objects[objIndex] = new BaseTileModel.ObjectTransitionState
            {
                Object = Model,
                State = TransitionState.Stationary
            };
            Model.coords = enteringTileCoords;    
            
            return true;

        }
        
        public bool TryStartMove(Direction direction, float speed)
        {
            Model.direction = direction;
            Model.speed = speed;
            
            var leavingTileCoords = new Vector2Int(Mathf.RoundToInt(Model.coords.x), Mathf.RoundToInt(Model.coords.y));
            var leavingTile = levelModel.Tiles[leavingTileCoords.y * levelModel.Data.fieldSize.x + leavingTileCoords.x];
            
            var objectIndex = leavingTile.objects.FindIndex(transition => transition.Object == Model);

            if (objectIndex == -1 || leavingTile.objects[objectIndex].Object == null || leavingTile.objects[objectIndex].State != TransitionState.Stationary)
            {
                Debug.LogError($"Object {Model} is absent in the tile {leavingTileCoords} or isn't stationary.");
                return false;
            }

            var enteringTileCoords = leavingTileCoords + Dir2Vel(direction);
            var enteringTile = levelModel.Tiles[enteringTileCoords.y * levelModel.Data.fieldSize.x + enteringTileCoords.x];

            if (!enteringTile.AllowObject(Model.Data.objectType))
            {
                return false;
            }
            
            leavingTile.objects[objectIndex] = new BaseTileModel.ObjectTransitionState
            {
                Object = Model,
                State = TransitionState.Leaving
            };
            
            enteringTile.objects.Add(new BaseTileModel.ObjectTransitionState
            {
                Object = Model,
                State = TransitionState.Entering
            });
            
            Model.coords += (Vector2)Dir2Vel(direction) * excess;
            excess = 0;

            return true;
        }

        private static Vector2Int Dir2Vel(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    return Vector2Int.up;
                case Direction.Down:
                    return Vector2Int.down;
                case Direction.Left:
                    return Vector2Int.left;
                case Direction.Right:
                    return Vector2Int.right;
                default:
                    return Vector2Int.zero;
            }
        }
        
        public void Dispose()
        {
            Model.Data.UnmarkAsInjected(this);
            Model.Data.UnbindObject(this);
            Model = null;
            levelModel = null;
        }
    }
}