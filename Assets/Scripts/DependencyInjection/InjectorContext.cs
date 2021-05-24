using System;
using System.Collections;
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
        private const int InjectionListCapacity = 5;
        
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
        private bool initialized;
        
        protected virtual void PreInject()
        {
            initialized = true;

            if (ParentContext == null && this != BaseContext)
            {
                ParentContext = BaseContext;
            }
            
            if (ParentContext != null)
            {
                ParentContext.childContexts.Add(this);

                if (!ParentContext.initialized)
                {
                    ParentContext.PreInject();
                }
            }
        }
        
        public ProxyBinding<T> Bind<T>() where T: class
        {
            var type = typeof(T);
            var binding = ObjectPool<ProxyBinding<T>>.Get().Update(type, this);
            
            Bind(binding);

            return binding;
        }
        
        public ProxyBinding<T> BindGameObject<T>() where T: Object
        {
            var type = typeof(T);
            var binding = ObjectPool<ProxyGameObjectBinding<T>>.Get().Update(type, this);
            
            Bind(binding);

            return binding;
        }
        
        public ProxyBinding<T> BindScriptableObject<T>() where T: ScriptableObject
        {
            var type = typeof(T);
            var binding = ObjectPool<ProxyScriptableObjectBinding<T>>.Get().Update(type, this);
            
            Bind(binding);

            return binding;
        }

        public void Bind<T>([NotNull] IBindable<T> binding) where T : class
        {
            var type = typeof(T);
            List<IBindable<object>> list;

            if (!injections.ContainsKey(type))
            {
                list = injections[type] = new List<IBindable<object>>(InjectionListCapacity);
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
            list.ForEach(binding => binding.Dispose());
            list.Clear();
        }
        
        public void Unbind<T>([NotNull] IBindable<T> binding) where T: class
        {
            var type = typeof(T);
            List<IBindable<object>> list;
            
            if (injections.ContainsKey(type) && (list = injections[type]).Contains(binding))
            {
                list.Remove(binding);
                binding.Dispose();
            }
            else
            {
                Debug.LogError("No binding found");
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

        public void RemoveTemporaryBindings<T>()
        {
            injections[typeof(T)].RemoveAll(inj => inj.Temporary);
        }
        
        public void Inject(object target)
        {
            if (!initialized)
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
        }

        private bool InjectField(object target, FieldInfo field)
        {
            IBindable<object> selectedInjection = null;

            if (injections.ContainsKey(field.FieldType))
            {
                foreach (var injection in injections[field.FieldType].Where(injection => injection.CheckConditions(target, field)))
                {
                    field.SetValue(target, injection.Inject());

                    if (target is IInjectable injectable)
                    {
                        injectable.OnInjected();
                    }
                
                    selectedInjection = injection;
                    break;
                }
            }
            
            if (selectedInjection != null || field.GetCustomAttribute<InjectAttribute>().Optional)
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
            initialized = false;
        }
    }
    
}