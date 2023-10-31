using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                //Search for an existing instance in the scene
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    //If no instance exists, create a new GameObject and add the component
                    GameObject singletonObject = new GameObject(typeof(T).Name);
                    instance = singletonObject.AddComponent<T>();
                }

                //Don't destroy the instance when loading new scenes
                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
}