using System.Collections.Generic;
using UnityEngine;
using Variecs.ProjectDII.Core.Level.Objects;
using Variecs.ProjectDII.DependencyInjection;

namespace Variecs.ProjectDII.Core.Level.LevelEditor
{
    public class ObjectSelectionView : MonoBehaviour, IInjectable
    {
        [InjectList] [SerializeField] private List<BaseObjectData> objectDataList;
        [Inject] private IFactory<ObjectSelectionItemView, ObjectSelectionItemView.Package> itemFactory;
        [SerializeField] private Transform itemContainer;
        [SerializeField] private ObjectSelectionItemView itemPrefab;
        
        public void OnInjected()
        {
            if (itemContainer == null)
            {
                itemContainer = transform;
            }
            
            foreach (var objectData in objectDataList)
            {
                itemFactory.GetInstance(new ObjectSelectionItemView.Package
                {
                    ObjectData = objectData,
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