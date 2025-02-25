using UnityEngine;

public class camera : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int width = Screen.width;
        int height = Screen.height;

        if (width % 2 != 0) width++;  // Make width even
        if (height % 2 != 0) height++; // Make height even

        Screen.SetResolution(width, height, Screen.fullScreen);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
