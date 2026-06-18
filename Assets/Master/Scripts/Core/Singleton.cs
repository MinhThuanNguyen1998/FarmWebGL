using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour
    where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError(
                    $"[Singleton<{typeof(T).Name}>] Instance is NULL. " +
                    $"Ensure exactly one instance exists before accessing Instance."
                );
            }
            return _instance;
        }
    }

    /// <summary>
    /// Có giữ lại khi load scene không
    /// </summary>
    protected bool IsPersistent = false;

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            // Duplicate → huỷ ngay
            Destroy(gameObject);
            return;
        }

        _instance = this as T;

        if (IsPersistent)
        {
            DontDestroyOnLoad(gameObject);
        }

        OnSingletonAwake();
    }

    /// <summary>
    /// Hook cho class con (thay vì override Awake)
    /// </summary>
    protected virtual void OnSingletonAwake() { }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
