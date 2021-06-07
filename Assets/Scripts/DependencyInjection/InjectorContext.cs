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

            foreach (var bindable in tempList)
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
            Bind(typeof(T), binding);
        }
        
        public void Bind(Type type, [NotNull] IBindable<object> binding)
        {
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
            
            foreach (var field in fields.Where(f => f.GetCustomAttributes<InjectListAttribute>().Any()))
            {
                if (!InjectListField(target, field))
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
            var result = false;

            if (injections.ContainsKey(field.FieldType))
            {
                foreach (var injection in injections[field.FieldType].Where(injection => injection.CheckConditions(target, field)))
                {
                    field.SetValue(target, injection.Inject());
                    result = true;
                    break;
                }
            }
            
            if (!result && ParentContext != null)
            {
                result = ParentContext.InjectField(target, field);
            }
            
            if (result || (field.GetCustomAttribute<InjectAttribute>()?.Optional ?? false))
            {
                return true;
            }
            
            Debug.LogError($"No bindings found in context {this} for type {field.FieldType} when injecting into mandatory field {field.Name} of object {target}");
            return false;
        }
        
        private bool InjectListField(object target, FieldInfo field)
        {
            if (!((field.GetValue(target) ?? Activator.CreateInstance(field.FieldType)) is IList list))
            {
                Debug.LogError($"Target type {field.FieldType} doesn't implement IList interface. Cannot inject list values in {target}.");
                return false;
            }
            
            field.SetValue(target, list);

            var subType = field.FieldType.GetGenericArguments()[0];
            
            if (injections.ContainsKey(subType))
            {
                foreach (var injection in injections[subType].Where(injection => injection.CheckConditions(target, field)))
                {
                    list.Add(injection.Inject());
                }
            }
            
            if (ParentContext != null)
            {
                ParentContext.InjectListField(target, field);
            }

            return true;
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