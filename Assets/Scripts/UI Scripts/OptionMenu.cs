using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    void Start()
    {
        if (AudioManager.instance == null)
        {
            // Debug.LogError("AudioManager instance not found!");
            return;
        }
        // Debug.Log("OptionsMenu: AudioManager instance found");

        // Temporarily removing listeners to avoid triggering the callbacks
        musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChange);
        sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChange);
        musicToggle.onValueChanged.RemoveListener(OnToggleMusic);
        sfxToggle.onValueChanged.RemoveListener(OnToggleSFX);

        // Loading saved preferences
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        sfxToggle.isOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        // Applying the loaded values directly to AudioManager without saving them again
        AudioManager.instance.SetMusicVolume(musicSlider.value);
        AudioManager.instance.SetSFXVolume(sfxSlider.value);
        AudioManager.instance.ToggleMusic(musicToggle.isOn);
        AudioManager.instance.ToggleSFX(sfxToggle.isOn);

        // Re-adding the listeners
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChange);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChange);
        musicToggle.onValueChanged.AddListener(OnToggleMusic);
        sfxToggle.onValueChanged.AddListener(OnToggleSFX);
    }

    public void OnMusicVolumeChange(float value)
    {
        // Debug.Log($"🔹 OnMusicVolumeChange() called with: {value}");
        // Debug.Log($"🔹 Current Slider Value: {musicSlider.value}");


        PlayerPrefs.SetFloat("MusicVolume", value);  // Saving the new value
        PlayerPrefs.Save();
        AudioManager.instance.SetMusicVolume(value);

    }

    public void OnSFXVolumeChange(float value)
    {
        // Debug.Log($"🔹 OnSFXVolumeChange() called with: {value}");
        // Debug.Log($"🔹 Current SFX Slider Value: {sfxSlider.value}");
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        AudioManager.instance.SetSFXVolume(value);

    }

    public void OnToggleMusic(bool enabled)
    {
        // Debug.Log($"🔹 OnToggleMusic() called with: {enabled}");
        // Debug.Log($"🔹 Current Music Toggle State: {musicToggle.isOn}");

        PlayerPrefs.SetInt("MusicEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
        AudioManager.instance.ToggleMusic(enabled);
    }

    public void OnToggleSFX(bool enabled)
    {
        // Debug.Log($"🔹 OnToggleSFX() called with: {enabled}");
        // Debug.Log($"🔹 Current SFX Toggle State: {sfxToggle.isOn}");

        PlayerPrefs.SetInt("SFXEnabled", enabled ? 1 : 0);
        PlayerPrefs.Save();
        AudioManager.instance.ToggleSFX(enabled);
    }
}
