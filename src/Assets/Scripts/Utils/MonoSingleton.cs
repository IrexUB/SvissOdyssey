using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T Pinstance;

    public static T instance { get { return Pinstance; } }
    public static bool IsInitialized { get { return Pinstance ? true : false; } }

    protected virtual void Awake()
    {
        if (Pinstance != null)
        {
            Debug.LogError("[Singleton] Trying to instantiate a second instance of a singleton class.");

            Destroy(gameObject);
            return;
        }

        Pinstance = (T)this;
        DontDestroyOnLoad(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (Pinstance == (T)this)
            Pinstance = null;
    }
}
