using System;
using System.Collections.Generic;

namespace Variecs.ProjectDII.DependencyInjection
{
    public static class SingletonHolder
    {
        private static Dictionary<Type, object> objectDictionary = new Dictionary<Type, object>();

        public static T Get<T>() where T: new()
        {
            var type = typeof(T);

            if (!objectDictionary.ContainsKey(type))
            {
                objectDictionary.Add(type, new T());
            }

            return (T)objectDictionary[type];
        }
        
        public static void Clear()
        {
            objectDictionary.Clear();
        }
    }
}