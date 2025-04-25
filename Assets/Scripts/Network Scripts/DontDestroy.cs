using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private void awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}