using UnityEngine;

public class Dont_distroy_sm : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
