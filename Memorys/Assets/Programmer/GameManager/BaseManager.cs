using UnityEngine;

public class BaseManager<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    public static T Instance
    {
        get
        {
            //なかったら探す
            if (instance == null) instance = (T)FindObjectOfType(typeof(T));
            return instance;
        }
    }
}
