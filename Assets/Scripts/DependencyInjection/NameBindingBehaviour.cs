using UnityEngine;

namespace Variecs.ProjectDII.DependencyInjection
{
    public class NameBindingBehaviour : MonoBehaviour
    {
        [SerializeField] protected InjectorContext injectionContext;
        [SerializeField] protected string injectionName;

        protected void Awake()
        {
            if (injectionContext == null)
            {
                injectionContext = InjectorContext.BaseContext;
            }

            if (string.IsNullOrEmpty(injectionName))
            {
                injectionName = name;
            }

            injectionContext.Bind<Transform>().ToValue(transform).ForName(injectionName);
            injectionContext.Bind<GameObject>().ToValue(gameObject).ForName(injectionName);
        }
    }
}