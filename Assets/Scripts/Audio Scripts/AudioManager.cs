using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Scene OSTs")]
    public AudioClip mainMenuOST;
    public AudioClip countrysideOST;
    public AudioClip desertOST;
    public AudioClip cityOST;  // Has a custom loop point
    public AudioClip hubDansOST;
    public AudioClip dansShopOST;
    public AudioClip battleOST;
    public AudioClip miniGameOST;

    [Header("Ambient SFX")]
    public AudioClip forestAmbienceSFX;
    public AudioClip desertAmbienceSFX;
    public AudioClip cityAmbienceSFX;

    [Header("Sound Effects (SFX)")]
    public AudioClip buttonClickSFX;
    public AudioClip buttonHoverSFX;
    public AudioClip impossibleActionSFX;
    public AudioClip countrysideWalkSFX;
    public AudioClip desertWalkSFX;
    public AudioClip cityWalkSFX;
    public AudioClip correctAnswerSFX;
    public AudioClip damageTakenSFX; // Wrong answer
    public AudioClip soulShatterSFX; // Death (game over)
    public AudioClip itemEffectSFX;
    public AudioClip diceRollSFX;
    
    private AudioSource bgmSource;
    private AudioSource sfxSource;
    private AudioSource ambientSource;
    private Coroutine cityLoopCoroutine;
    private Coroutine resumeMusicCoroutine;

    private AudioClip currentSceneMusic;
    private bool isCityScene = false;
    private float cityLoopStartTime = 15f;

    // Volume settings
    private float musicVolume = 1f;
    private float sfxVolume = 1f;
    private bool isMusicEnabled = true;
    private bool isSFXEnabled = true;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create audio sources
        bgmSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();
        ambientSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        sfxSource.loop = false;
        ambientSource.loop = true;

        LoadAudioSettings();
    }

    void Start()
    {
        PlayMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    void PlayMusicForScene(string sceneName)
    {
        AudioClip newClip = null;
        AudioClip newAmbience = null;
        isCityScene = false;

        switch (sceneName)
        {
            case "MainMenu":
                newClip = mainMenuOST;
                break;
            case "CountrySide":
                newClip = countrysideOST;
                newAmbience = forestAmbienceSFX;
                break;
            case "Desert":
                newClip = desertOST;
                newAmbience = desertAmbienceSFX;
                break;
            case "City":
                newClip = cityOST;
                newAmbience = cityAmbienceSFX;
                isCityScene = true;
                break;
            case "HubDans":
                newClip = hubDansOST;
                newAmbience = desertAmbienceSFX;
                break;
        }

        // Handle BGM
        if (newClip != null && bgmSource.clip != newClip)
        {
            if (cityLoopCoroutine != null) StopCoroutine(cityLoopCoroutine);
            if (resumeMusicCoroutine != null) StopCoroutine(resumeMusicCoroutine);

            currentSceneMusic = newClip;
            bgmSource.clip = newClip;

            if (isMusicEnabled) bgmSource.Play();

            if (isCityScene)
            {
                bgmSource.loop = false;
                cityLoopCoroutine = StartCoroutine(HandleCityLoop());
            }
            else
            {
                bgmSource.loop = true;
            }
        }

        // Handle Ambience SFX
        if (newAmbience != null && ambientSource.clip != newAmbience)
        {
            ambientSource.clip = newAmbience;
            if (isSFXEnabled) ambientSource.Play();
        }
        else if (newAmbience == null)
        {
            ambientSource.Stop();
        }
    }

    private IEnumerator HandleCityLoop()
    {
        while (true)
        {
            yield return null;
            if (bgmSource.time >= bgmSource.clip.length - 0.1f)
            {
                bgmSource.time = cityLoopStartTime;
                bgmSource.Play();
            }
        }
    }

    public void PlayTemporaryMusic(AudioClip tempClip, bool waitForCompletion = true)
    {
        if (tempClip == null || !isMusicEnabled) return;
        if (bgmSource.clip == tempClip) return;

        if (cityLoopCoroutine != null) StopCoroutine(cityLoopCoroutine);
        if (resumeMusicCoroutine != null) StopCoroutine(resumeMusicCoroutine);

        StartCoroutine(SwitchToTemporaryMusic(tempClip, waitForCompletion));
    }

    private IEnumerator SwitchToTemporaryMusic(AudioClip tempClip, bool waitForCompletion)
    {
        float savedTime = bgmSource.time;
        bgmSource.Stop();
        bgmSource.clip = tempClip;
        bgmSource.loop = false;
        bgmSource.Play();

        if (waitForCompletion)
        {
            yield return new WaitWhile(() => bgmSource.isPlaying);
            ResumeSceneMusic(savedTime);
        }
    }

    public void ResumeSceneMusic(float resumeTime = 0f)
    {
        if (currentSceneMusic == null || !isMusicEnabled) return;

        bgmSource.clip = currentSceneMusic;
        bgmSource.time = resumeTime;
        bgmSource.Play();

        if (isCityScene)
        {
            bgmSource.loop = false;
            cityLoopCoroutine = StartCoroutine(HandleCityLoop());
        }
        else
        {
            bgmSource.loop = true;
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || !isSFXEnabled) return;
        sfxSource.PlayOneShot(clip, volume * sfxVolume);
    }

    // ====== Volume and Toggle Settings ======
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        bgmSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ambientSource.volume = sfxVolume; // Ensure ambient volume follows SFX volume
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void ToggleMusic(bool isEnabled)
    {
        isMusicEnabled = isEnabled;
        PlayerPrefs.SetInt("MusicEnabled", isMusicEnabled ? 1 : 0);
        PlayerPrefs.Save();

        if (isMusicEnabled)
            ResumeSceneMusic();
        else
            bgmSource.Stop();
    }

    public void ToggleSFX(bool isEnabled)
    {
        isSFXEnabled = isEnabled;
        PlayerPrefs.SetInt("SFXEnabled", isSFXEnabled ? 1 : 0);
        PlayerPrefs.Save();

        if (isSFXEnabled)
            ambientSource.Play();
        else
            ambientSource.Stop();
    }

    private void LoadAudioSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        isMusicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        isSFXEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        bgmSource.volume = musicVolume;
        ambientSource.volume = sfxVolume;
    }
}