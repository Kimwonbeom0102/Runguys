/*using UnityEngine;

namespace Practices.UGUI_Management.Singletons
{
    // [CHANGE] 기존 Activator.CreateInstance 방식을 제거하고, Unity 전용 싱글톤 패턴으로 수정
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T s_instance;
        public static T instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = FindObjectOfType<T>();
                    if (s_instance == null)
                    {
                        // [ADDED] 씬에 존재하지 않으면 새 GameObject를 생성하여 추가
                        GameObject singletonObj = new GameObject(typeof(T).Name);
                        s_instance = singletonObj.AddComponent<T>();
                        DontDestroyOnLoad(singletonObj);
                    }
                }
                return s_instance;
            }
        }

        protected virtual void Awake()
        {
            if (s_instance == null)
            {
                s_instance = this as T;
                DontDestroyOnLoad(gameObject); // [ADDED] 씬 전환 시 유지
            }
            else if (s_instance != this)
            {
                Destroy(gameObject);
            }
        }
    }
}*/



using System;
using System.Reflection;

namespace Practices.UGUI_Management.Singletons
{
    /// <summary>
    /// Singleton base
    /// </summary>
    /// <typeparam name="T"> 싱글톤으로 사용하려는 타입 (상속클래스) </typeparam>
    public abstract class Singleton<T>
        where T : Singleton<T>
    {
        public static T instance
        {
            get
            {
                if (s_instance == null)
                {
                    //ConstructorInfo constructorInfo = typeof(T).GetConstructor(new Type[] { });
                    //s_instance = (T)constructorInfo.Invoke(null);

                    s_instance = (T)Activator.CreateInstance(typeof(T));
                }

                return s_instance;
            }
        }


        private static T s_instance;
    }
}