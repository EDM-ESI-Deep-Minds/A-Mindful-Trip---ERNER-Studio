using UnityEngine;

public class MicAccess : MonoBehaviour
{
    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            string mic = Microphone.devices[0];
            Microphone.Start(mic, false, 1, 44100);
            Debug.Log("Microphone access requested.");
        }
        else
        {
            Debug.LogWarning("No microphone detected.");
        }
    }
}