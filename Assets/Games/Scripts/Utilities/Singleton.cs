using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected bool _dontDestroyOnLoad = true;

    /// <summary>
    /// Private static instance.
    /// </summary>
    static T _instance;

    /// <summary>
    /// Public static instance used to refer to Singleton (Eg: SomeClass.Instance)
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    GameObject singleton = new GameObject(typeof(T).Name);
                    _instance = singleton.AddComponent<T>();
                    DontDestroyOnLoad(singleton);
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        GetInstance(true);
    }

    public void GetInstance(bool dontDestroyOnload)
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (dontDestroyOnload)
            {
                var root = transform.root;

                if (root != transform)
                {
                    if (_dontDestroyOnLoad) DontDestroyOnLoad(root);
                }
                else
                {
                    if (_dontDestroyOnLoad) DontDestroyOnLoad(this.gameObject);
                }
            }
        }
        else
        {
            Debug.LogWarning($"An instance of <color=#26ffac><b>{gameObject.name}</b></color> has been destroyed.");
            Destroy(gameObject);
        }
    }
}