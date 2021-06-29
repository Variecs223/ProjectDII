using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Objects;
using Variecs.ProjectDII.Core.Level.Tiles;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.LevelEditor
{
    public class TileSelectionView : MonoBehaviour, IInjectable
    {
        [Inject] [SerializeField] private List<TileType> tileTypeList;
        [Inject] private IFactory<TileSelectionItemView, TileSelectionItemView.Package> itemFactory;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private TileSelectionItemView itemPrefab;
        
        public void OnInjected()
        {
            foreach (var tileType in tileTypeList)
            {
                itemFactory.GetInstance(new TileSelectionItemView.Package
                {
                    Type = tileType,
                    Container = itemContainer,
                    Prefab = itemPrefab
                });
            }
        }

        public void Dispose()
        {
            
        }
    }
}