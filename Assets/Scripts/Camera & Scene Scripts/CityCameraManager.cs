using UnityEngine;
using UnityEngine.U2D; 

public class CityCameraManager : MonoBehaviour
{
    private PixelPerfectCamera pixelPerfectCamera;

    void Start()
    {
        pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

        if (pixelPerfectCamera != null)
        {
            pixelPerfectCamera.refResolutionY = 360; // Adjust for zoom out
        }
    }
}
