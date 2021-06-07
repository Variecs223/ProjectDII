﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Objects;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level
{
    [Serializable]
    public class LevelModel: IModel, IInjectable
    {
        [Inject] [SerializeField] protected LevelData data;
        [Inject] private IFactory<BaseTileModel, TileType> tileFactory;
        [Inject] private IFactory<IObjectPackage, ObjectType> objectFactory;

        public BaseTileModel[] tiles;
        public List<BaseObjectModel> objects;
        public LevelData.ActionCategory[] actions;
        public int selectedAction;
        
        public LevelData Data => data;
        InjectorContext IModel.ModelType => Data;
        public event Action OnRemoved;

        public event Action<IObjectPackage> OnObjectAdded;

        public void OnInjected()
        {
            
        }

        public void Load()
        {
            tiles = new BaseTileModel[Data.tiles.Length];

            for (var i = 0; i < tiles.Length; i++)
            {
                tiles[i] = tileFactory.GetInstance(Data.tiles[i]);
            }

            objects = new List<BaseObjectModel>();
            
            foreach (var objectLocation in Data.objects)
            {
                AddObject(objectLocation.Type, objectLocation.Coords, objectLocation.Direction);
            }

            actions = Data.actions.ToArray();
        }

        public void AddObject(ObjectType type, Vector2Int coords, Direction dir = Direction.Up)
        {
            using var package = objectFactory.GetInstance(type);
            
            AddObject(package, coords, dir);
        }

        public void AddObject(IObjectPackage package, Vector2Int coords, Direction dir = Direction.Up)
        {
            package.GetModels(model =>
            {
                model.coords = coords + Vector2.one * 0.5f;
                model.direction = dir;
                objects.Add(model);
                tiles[coords.y * Data.fieldSize.x + coords.x].objects.Add(
                    new BaseTileModel.ObjectTransitionState
                    {
                        Object = model,
                        State = TransitionState.Stationary
                    });
            });
                
            OnObjectAdded?.Invoke(package);
        }

        public void RemoveObject(BaseObjectModel model)
        {
            if (objects.Contains(model))
            {
                objects.Remove(model);
            }

            var coords = new Vector2Int(Mathf.FloorToInt(model.coords.x), Mathf.FloorToInt(model.coords.y));
            var tileObjects = tiles[coords.y * Data.fieldSize.x + coords.x].objects;
            var objectIndex = tileObjects.FindIndex(obj => obj.Object == model);

            if (objectIndex == -1 || tileObjects[objectIndex].Object == null)
            {
                Debug.LogError($"Didn't find object {model} in tile {coords} when trying to delete it");
                return;
            }

            if (tileObjects[objectIndex].State == TransitionState.Stationary)
            {
                tileObjects.RemoveAt(objectIndex);
                return;
            }

            var secondCoords = coords;

            if (tileObjects[objectIndex].State == TransitionState.Leaving)
            {
                secondCoords += MovableController.Dir2Vel(model.direction);
            }
            else
            {
                secondCoords -= MovableController.Dir2Vel(model.direction);
            }

            var secondTileObjects = tiles[secondCoords.y * Data.fieldSize.x + secondCoords.x].objects;
            var secondObjectIndex = secondTileObjects.FindIndex(obj => obj.Object == model);
            
            tileObjects.RemoveAt(objectIndex);
            secondTileObjects.RemoveAt(secondObjectIndex);
            
            model.Dispose();
        }
        
        public void Dispose()
        {
            if (Data != null)
            {
                Data.UnmarkAsInjected(this);
            }

            if (tiles != null)
            {
                foreach (var tile in tiles)
                {
                    tile.Dispose();
                } 
            }

            if (objects != null)
            {
                foreach (var obj in objects)
                {
                    obj.Dispose();
                }
            }
            
            tiles = null;
            objects = null;
            actions = null;
            
            OnRemoved?.Invoke();
        }
    }
}