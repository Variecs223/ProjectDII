using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Variecs.ProjectDII.DependencyInjection.Bindables;
using Object = UnityEngine.Object;

namespace Variecs.ProjectDII.DependencyInjection
{
    [CreateAssetMenu(fileName = "InjectorContext", menuName = "DII/Dependency Injection/Injector Context", order = 0)]
    public class InjectorContext : ScriptableObject, IDisposable
    {
        private static InjectorContext baseContext;

        public static InjectorContext BaseContext
        {
            get
            {
                if (baseContext == null)
                {
                    baseContext = Resources.Load<InjectorContext>("BaseContext");
                }

                return baseContext;
            }
        }
        
        [SerializeField] private InjectorContext parentContext;
        public InjectorContext ParentContext
        {
            get => parentContext;
            set
            {
                if (parentContext != null && parentContext.ChildContexts.Contains(this))
                {
                    parentContext.childContexts.Remove(this);
                }
                
                parentContext = value;
                
                parentContext.childContexts.Add(this);
            }
        }

        private readonly List<InjectorContext> childContexts = new List<InjectorContext>();
        public IReadOnlyList<InjectorContext> ChildContexts => childContexts;

        private readonly Dictionary<Type, List<IBindable<object>>> injections = new Dictionary<Type, List<IBindable<object>>>();

        private readonly HashSet<object> injectedObjects = new HashSet<object>();
        private readonly Dictionary<object, IList<IBindable<object>>> objectBindings = new Dictionary<object, IList<IBindable<object>>>();
        private readonly Dictionary<GameObject, IList<IBindable<object>>> gameObjectBindings = new Dictionary<GameObject, IList<IBindable<object>>>();
        
        protected bool Initialized;

        public void Init()
        {
            Inject(this);
        }
        
        protected virtual void PreInject()
        {
            Initialized = true;

            if (ParentContext == null && this != BaseContext)
            {
                ParentContext = BaseContext;
            }
            
            if (ParentContext != null)
            {
                ParentContext.childContexts.Add(this);

                if (!ParentContext.Initialized)
                {
                    ParentContext.PreInject();
                }
            }
        }
        
        public void RegisterObjectBinding(object target, IBindable<object> bindable)
        {
            if (!objectBindings.ContainsKey(target))
            {
                objectBindings[target] = ObjectPool<List<IBindable<object>>>.Get();
            }

            objectBindings[target].Add(bindable);
        }

        public void RegisterGameObjectBinding(GameObject target, IBindable<object> bindable)
        {
            if (!gameObjectBindings.ContainsKey(target))
            {
                gameObjectBindings[target] = ObjectPool<List<IBindable<object>>>.Get();
            }

            gameObjectBindings[target].Add(bindable);
        }
        
        public void UnregisterObjectBinding(object target, IBindable<object> bindable)
        {
            if (objectBindings.ContainsKey(target) && objectBindings[target].Contains(bindable))
            {
                objectBindings[target].Remove(bindable);
            }
        }

        public void UnregisterGameObjectBinding(GameObject target, IBindable<object> bindable)
        {
            if (gameObjectBindings.ContainsKey(target) && gameObjectBindings[target].Contains(bindable))
            {
                gameObjectBindings[target].Remove(bindable);
            }
        }

        public void UnbindObject(object target)
        {
            if (!objectBindings.ContainsKey(target))
            {
                return;
            }

            var tempList = ObjectPool<List<IBindable<object>>>.Get();
            tempList.AddRange(objectBindings[target]);

            foreach (var bindable in tempList)
            {
                Unbind(bindable);
            }
            
            tempList.Clear();
            ObjectPool<List<IBindable<object>>>.Put(tempList);
        }

        public void UnbindGameObject(GameObject target)
        {
            if (!gameObjectBindings.ContainsKey(target))
            {
                return;
            }
            
            var tempList = ObjectPool<List<IBindable<object>>>.Get();
            tempList.AddRange(gameObjectBindings[target]);

            foreach (var bindable in gameObjectBindings[target])
            {
                Unbind(bindable);
            }
            
            tempList.Clear();
            ObjectPool<List<IBindable<object>>>.Put(tempList);
        }
        
        public ProxyBinding<T> Bind<T>() where T: class
        {
            var binding = ObjectPool<ProxyBinding<T>>.Get().Update(this);
            
            Bind(binding);

            return binding;
        }
        
        public ProxyBinding<T> BindGameObject<T>() where T: Object
        {
            var binding = ObjectPool<ProxyGameObjectBinding<T>>.Get().Update(this);
            
            Bind(binding);

            return binding;
        }
        
        public ProxyBinding<T> BindScriptableObject<T>() where T: ScriptableObject
        {
            var binding = ObjectPool<ProxyScriptableObjectBinding<T>>.Get().Update(this);
            
            Bind(binding);

            return binding;
        }

        public void Bind<T>([NotNull] IBindable<T> binding) where T : class
        {
            var type = typeof(T);
            List<IBindable<object>> list;

            if (!injections.ContainsKey(type))
            {
                list = injections[type] = ObjectPool<List<IBindable<object>>>.Get();
            }
            else
            {
                list = injections[type];
            }
            
            list.Add(binding);
        }

        public void Unbind<T>() where T : class
        {
            var type = typeof(T);
            
            if (!injections.ContainsKey(type))
            {
                return;
            }
            
            var list = injections[type];

            foreach (var binding in list)
            {
                binding.Dispose();
            }

            list.Clear();
            ObjectPool<List<IBindable<object>>>.Put(list);
            injections.Remove(type);
        }

        public void Unbind([NotNull] IBindable<object> binding)
        {
            var type = binding.GetBindedType();
            List<IBindable<object>> list;
            
            if (injections.ContainsKey(type) && (list = injections[type]).Contains(binding))
            {
                list.Remove(binding);
                binding.Dispose();

                if (list.Any())
                {
                    return;
                }
                
                ObjectPool<List<IBindable<object>>>.Put(list);
                injections.Remove(type);
            }
        }

        public IBindable<T> FindBinding<T>(Predicate<IBindable<T>> predicate) where T: class
        {
            var type = typeof(T);
            
            if (injections.ContainsKey(type))
            {
                foreach (var binding in injections[type])
                {
                    if (binding is IBindable<T> concreteBinding && predicate.Invoke(concreteBinding))
                    {
                        return concreteBinding;
                    }
                }
            }

            return ParentContext == null ? null : ParentContext.FindBinding(predicate);
        }

        public void MarkAsInjected(object target)
        {
            injectedObjects.Add(target);

            if (ParentContext != null)
            {
                ParentContext.MarkAsInjected(target);
            }
        }
        
        public void UnmarkAsInjected(object target)
        {
            injectedObjects.Remove(target);
        }
        
        public void Inject(object target)
        {
            if (!Initialized)
            {
                PreInject();
            }

            if (injectedObjects.Contains(target))
            {
                return;
            }
            
            MarkAsInjected(target);
            
            var type = target.GetType();
            var fields = type.GetFields(BindingFlags.Instance | 
                                        BindingFlags.NonPublic |
                                        BindingFlags.Public);

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var field in fields.Where(f => f.GetCustomAttributes<InjectAttribute>().Any()))
            {
                if (!InjectField(target, field))
                {
                    return;
                }
            }
            
            if (target is IInjectable injectable)
            {
                injectable.OnInjected();
            }
        }

        private bool InjectField(object target, FieldInfo field)
        {
            IBindable<object> selectedInjection = null;

            if (injections.ContainsKey(field.FieldType))
            {
                foreach (var injection in injections[field.FieldType].Where(injection => injection.CheckConditions(target, field)))
                {
                    field.SetValue(target, injection.Inject());

                    selectedInjection = injection;
                    break;
                }
            }

            var result = selectedInjection != null;
            
            if (!result && ParentContext != null)
            {
                result = ParentContext.InjectField(target, field);
            }
            
            if (result || field.GetCustomAttribute<InjectAttribute>().Optional)
            {
                return true;
            }
            
            Debug.LogError($"No bindings found for type {field.FieldType} when injecting into mandatory field {field.Name} of object {target}");
            return false;
        }
        
        public virtual void Dispose() 
        {

            if (ParentContext != null && ParentContext.ChildContexts.Contains(this))
            {
                ParentContext.childContexts.Remove(this);
            }
            
            injections.Clear();
            injectedObjects.Clear();
            Initialized = false;
        }
    }
    
}