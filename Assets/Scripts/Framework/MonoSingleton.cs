using UnityEngine;

namespace Framework
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        protected static T _instance = null;

        public static T Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
