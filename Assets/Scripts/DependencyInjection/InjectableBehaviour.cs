using UnityEngine;

namespace Variecs.ProjectDII.DependencyInjection
{
    public class InjectableBehaviour : MonoBehaviour, IInjectable
    {
        [SerializeField] private InjectorContext context;
        
        protected void Awake()
        {
            if (context == null)
            {
                context = InjectorContext.BaseContext;
            }
            
            context.Inject(this);
        }

        public void OnInjected()
        {
            
        }
    }
}