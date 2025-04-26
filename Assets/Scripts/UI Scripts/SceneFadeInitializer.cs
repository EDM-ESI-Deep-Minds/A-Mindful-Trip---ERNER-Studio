using UnityEngine;
using System.Collections;

public class SceneFadeInitializer : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(FadeScreen.Instance.FadeIn());
    }
}
