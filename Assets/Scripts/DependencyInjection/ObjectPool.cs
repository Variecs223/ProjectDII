using System.Collections.Generic;

namespace Variecs.ProjectDII.DependencyInjection
{
    public static class ObjectPool<T> where T: class, new()
    {
        private static readonly Stack<T> Pool = new Stack<T>();

        public static T Get()
        {
            return Pool.Count == 0 ? new T() : Pool.Pop();
        }

        public static void Put(T val)
        {
            if (Pool.Contains(val) || val == null)
            {
                return;
            }
            
            Pool.Push(val);
        }

        public static void Clear()
        {
            Pool.Clear();
        }
    }
}