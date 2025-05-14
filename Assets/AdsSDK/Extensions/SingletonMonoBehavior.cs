using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T ins;

    public virtual void Awake()
    {
        if (ins == null)
        {
            ins = this as T;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    public static bool Exists()
    {
        return (ins != null);
    }
}