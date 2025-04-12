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
    private AudioSource walkSource;

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
        walkSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
        sfxSource.loop = false;
        ambientSource.loop = true;
        walkSource.loop = true;
        walkSource.playOnAwake = false;

        LoadAudioSettings();
    }

    void Start()
    {
        // Only play music if it's enabled
        if (isMusicEnabled)
        {
            PlayMusicForScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            // Just setting the references without playing
            SetSceneMusicWithoutPlaying(SceneManager.GetActiveScene().name);
        }
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
        // Only playing music if it's enabled
        if (isMusicEnabled)
        {
            PlayMusicForScene(scene.name);
        }
        else
        {
            // Just setting the references without playing
            SetSceneMusicWithoutPlaying(scene.name);
        }
    }

    // Setting the scene music references without playing them
    void SetSceneMusicWithoutPlaying(string sceneName)
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
            case "Hub&Dans":
                newClip = hubDansOST;
                newAmbience = desertAmbienceSFX;
                break;
            case "DansShop":
                newClip = dansShopOST;
                break;
            case "Battle":
                newClip = battleOST;
                break;
            case "MiniGame":
                newClip = miniGameOST;
                break;
        }

        // Just updating references without playing
        if (newClip != null)
        {
            currentSceneMusic = newClip;
            bgmSource.clip = newClip;
        }

        // Updating ambient reference without playing
        if (newAmbience != null)
        {
            ambientSource.clip = newAmbience;
        }
    }

    void PlayMusicForScene(string sceneName)
    {
        // Debug.Log($"PlayMusicForScene called for {sceneName}. Music Enabled: {isMusicEnabled}");

        // Early return if music is disabled
        if (!isMusicEnabled)
        {
            SetSceneMusicWithoutPlaying(sceneName);
            return;
        }

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
            case "Hub&Dans":
                newClip = hubDansOST;
                newAmbience = desertAmbienceSFX;
                break;
            case "DansShop":
                newClip = dansShopOST;
                break;
            case "Battle":
                newClip = battleOST;
                break;
            case "MiniGame":
                newClip = miniGameOST;
                break;
        }

        // Handle BGM
        if (newClip != null)
        {
            // Storing current clip reference
            currentSceneMusic = newClip;

            // Only changing and play music if music is enabled
            if (isMusicEnabled)
            {
                // Stop existing coroutines
                if (cityLoopCoroutine != null)
                {
                    StopCoroutine(cityLoopCoroutine);
                    cityLoopCoroutine = null;
                }
                if (resumeMusicCoroutine != null)
                {
                    StopCoroutine(resumeMusicCoroutine);
                    resumeMusicCoroutine = null;
                }

                // Setting the new clip and play
                bgmSource.clip = newClip;
                bgmSource.volume = musicVolume;  // Ensuring volume is applied
                bgmSource.Play();

                // Handling city music looping
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
        }

        // Handling Ambient SFX - only if SFX is enabled
        if (newAmbience != null)
        {
            ambientSource.clip = newAmbience;
            ambientSource.volume = sfxVolume;  // Applying volume settings

            if (isSFXEnabled)
            {
                ambientSource.Play();
            }
            else
            {
                ambientSource.Stop();
            }
        }
        else if (newAmbience == null && ambientSource.isPlaying)
        {
            ambientSource.Stop();
        }
    }

    private IEnumerator HandleCityLoop()
    {
        while (bgmSource != null && bgmSource.isPlaying && isMusicEnabled)
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
        // Don't play temporary music if music is disabled
        if (tempClip == null || !isMusicEnabled) return;
        if (bgmSource.clip == tempClip) return;

        if (cityLoopCoroutine != null)
        {
            StopCoroutine(cityLoopCoroutine);
            cityLoopCoroutine = null;
        }
        if (resumeMusicCoroutine != null)
        {
            StopCoroutine(resumeMusicCoroutine);
            resumeMusicCoroutine = null;
        }

        resumeMusicCoroutine = StartCoroutine(SwitchToTemporaryMusic(tempClip, waitForCompletion));
    }

    private IEnumerator SwitchToTemporaryMusic(AudioClip tempClip, bool waitForCompletion)
    {
        if (!isMusicEnabled) yield break;

        float savedTime = bgmSource.time;
        bgmSource.Stop();
        bgmSource.clip = tempClip;
        bgmSource.volume = musicVolume;  // Ensuring volume is applied
        bgmSource.loop = false;
        bgmSource.Play();

        if (waitForCompletion)
        {
            yield return new WaitWhile(() => bgmSource.isPlaying && isMusicEnabled);

            // Only resuming scene music if music is still enabled
            if (isMusicEnabled)
            {
                ResumeSceneMusic(savedTime);
            }
        }
    }

    public void ResumeSceneMusic(float resumeTime = 0f)
    {
        // Don't resume if music is disabled or no current music
        if (currentSceneMusic == null || !isMusicEnabled) return;

        bgmSource.clip = currentSceneMusic;
        bgmSource.volume = musicVolume;  // Ensure volume is applied
        bgmSource.time = resumeTime;

        // Only actually play if music is enabled
        if (isMusicEnabled)
        {
            bgmSource.Play();

            if (isCityScene)
            {
                bgmSource.loop = false;

                // Cleanig up any existing coroutine
                if (cityLoopCoroutine != null)
                {
                    StopCoroutine(cityLoopCoroutine);
                }

                cityLoopCoroutine = StartCoroutine(HandleCityLoop());
            }
            else
            {
                bgmSource.loop = true;
            }
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || !isSFXEnabled) return;
        // Debug.Log($"Playing SFX with volume: {sfxVolume * volume}");
        sfxSource.PlayOneShot(clip, volume * sfxVolume);
    }

    // public void PlayWalkSFXForScene()
    // {
    //     if (!isSFXEnabled) return;

    //     string scene = SceneManager.GetActiveScene().name;

    //     switch (scene)
    //     {
    //         case "CountrySide":
    //         case "Hub&Dans":
    //             PlaySFX(countrysideWalkSFX);
    //             break;
    //         case "Desert":
    //             PlaySFX(desertWalkSFX);
    //             break;
    //         case "City":
    //             PlaySFX(cityWalkSFX);
    //             break;
    //     }
    // }

    public void StartWalkingLoop()
    {
        if (!isSFXEnabled) return;

        AudioClip walkClip = null;
        string scene = SceneManager.GetActiveScene().name;

        switch (scene)
        {
            case "CountrySide":
            case "Hub&Dans":
                walkClip = countrysideWalkSFX;
                break;
            case "Desert":
                walkClip = desertWalkSFX;
                break;
            case "City":
                walkClip = cityWalkSFX;
                break;
        }

        if (walkClip != null && walkSource.clip != walkClip)
        {
            walkSource.clip = walkClip;
        }

        if (!walkSource.isPlaying)
        {
            walkSource.volume = sfxVolume;
            walkSource.Play();
        }
    }

    public void StopWalkingLoop()
    {
        if (walkSource.isPlaying)
        {
            walkSource.Stop();
        }
    }



    // ====== Volume and Toggle Settings ======
    public void SetMusicVolume(float volume)
    {
        // Debug.Log($"Music volume set to: {volume}");

        musicVolume = Mathf.Clamp01(volume);

        // Directly applying to bgmSource
        if (bgmSource != null)
        {
            bgmSource.volume = musicVolume;
        }

        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        // Debug.Log($"SFX volume set to: {volume}");

        sfxVolume = Mathf.Clamp01(volume);

        // Applying to both sfx sources
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }

        if (ambientSource != null)
        {
            ambientSource.volume = sfxVolume;
        }

        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void ToggleMusic(bool isEnabled)
    {
        // Debug.Log($"Music toggled to: {isEnabled}");

        isMusicEnabled = isEnabled;
        PlayerPrefs.SetInt("MusicEnabled", isMusicEnabled ? 1 : 0);
        PlayerPrefs.Save();

        // Handling music being toggled OFF
        if (!isMusicEnabled)
        {
            // Stopping all music-related processes
            StopAllCoroutines();
            bgmSource.Stop();
            cityLoopCoroutine = null;
            resumeMusicCoroutine = null;
            return; // Early exit
        }

        // Handling music being toggled ON
        if (currentSceneMusic != null)
        {
            bgmSource.clip = currentSceneMusic;
            bgmSource.volume = musicVolume;
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
    }

    public void ToggleSFX(bool isEnabled)
    {
        // Debug.Log($"SFX toggled to: {isEnabled}");

        isSFXEnabled = isEnabled;
        PlayerPrefs.SetInt("SFXEnabled", isSFXEnabled ? 1 : 0);
        PlayerPrefs.Save();

        // Handling SFX toggling
        if (isSFXEnabled)
        {
            // Only starting ambient if there's a clip assigned
            if (ambientSource.clip != null)
            {
                ambientSource.volume = sfxVolume;
                ambientSource.Play();
            }

            // Play a test sound to confirm SFX is working
            // if (buttonClickSFX != null)
            // {
            //     PlaySFX(buttonClickSFX, 1f);
            // }
        }
        else
        {
            // Stopping ambient sound
            ambientSource.Stop();
        }
    }

    private void LoadAudioSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        isMusicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        isSFXEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;

        // Applying loaded values directly to audio sources
        if (bgmSource != null)
        {
            bgmSource.volume = musicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }

        if (ambientSource != null)
        {
            ambientSource.volume = sfxVolume;
        }

        // If music should be disabled at startup, ensure it's not playing
        if (!isMusicEnabled && bgmSource != null && bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }

        // If SFX should be disabled at startup, ensure ambient isn't playing
        if (!isSFXEnabled && ambientSource != null && ambientSource.isPlaying)
        {
            ambientSource.Stop();
        }
    }
}