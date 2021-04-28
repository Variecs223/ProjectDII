using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine;
using Variecs.ProjectDII.Core.Bindables;
using Object = UnityEngine.Object;

namespace Variecs.ProjectDII.Core
{
    [CreateAssetMenu(fileName = "InjectorContext", menuName = "DII/Core/Injector Context", order = 0)]
    public class InjectorContext : ScriptableObject
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

        protected void Awake()
        {
            if (ParentContext != null)
            {
                ParentContext.childContexts.Add(this);
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
                list = injections[type] = new List<IBindable<object>>();
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
            List<IBindable<object>> list;

            if (!injections.ContainsKey(type))
            {
                list = injections[type] = new List<IBindable<object>>();
            }
            else
            {
                list = injections[type];
            }
            
            list.ForEach(binding => binding.Dispose());
            list.Clear();
        }
        
        public void Unbind<T>(IBindable<T> binding) where T: class
        {
            var type = typeof(T);
            List<IBindable<object>> list;

            if (!injections.ContainsKey(type))
            {
                list = injections[type] = new List<IBindable<object>>();
            }
            else
            {
                list = injections[type];
            }

            if (list.Contains(binding))
            {
                list.Remove(binding);
                binding.Dispose();
            }
            else
            {
                Debug.LogError("No binding found");
            }
        }

        public void Inject(object target)
        {
            var type = target.GetType();
            var fields = type.GetFields();

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
            if (!injections.ContainsKey(field.FieldType))
            {
                var result = ParentContext == null || ParentContext.InjectField(target, field);
                
                if (!result)
                {
                    Debug.LogError($"No bindings found for type {field.FieldType} when injecting into field {field.Name} of object {target}");
                }
                
                return result;
            }

            foreach (var injection in injections[field.FieldType])
            {
                field.SetValue(target, injection.Inject());
                return true;
            }
            
            Debug.LogError($"No matching bindings found for type {field.FieldType} when injecting into field {field.Name} of object {target}");
            return false;
        }
    }
}