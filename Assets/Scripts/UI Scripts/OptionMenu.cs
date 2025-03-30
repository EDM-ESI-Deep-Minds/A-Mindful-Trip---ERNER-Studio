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
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicToggle.isOn = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        sfxToggle.isOn = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
    }

    public void OnMusicVolumeChange(float value) => AudioManager.instance.SetMusicVolume(value);
    public void OnSFXVolumeChange(float value) => AudioManager.instance.SetSFXVolume(value);
    public void OnToggleMusic(bool enabled) => AudioManager.instance.ToggleMusic(enabled);
    public void OnToggleSFX(bool enabled) => AudioManager.instance.ToggleSFX(enabled);
}
