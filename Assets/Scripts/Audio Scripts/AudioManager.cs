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

    private AudioSource audioSource;
    private Coroutine cityLoopCoroutine;
    private Coroutine resumeMusicCoroutine;

    private AudioClip currentSceneMusic;
    private bool isCityScene = false;
    private float cityLoopStartTime = 15f; // Loop start time for city OST

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

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
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
        isCityScene = false;

        switch (sceneName)
        {
            case "MainMenu":
                newClip = mainMenuOST;
                break;
            case "CountrySide":
                newClip = countrysideOST;
                break;
            case "Desert":
                newClip = desertOST;
                break;
            case "City":
                newClip = cityOST;
                isCityScene = true;
                break;
            case "HubDans":
                newClip = hubDansOST;
                break;
        }

        if (newClip != null && audioSource.clip != newClip)
        {
            if (cityLoopCoroutine != null) StopCoroutine(cityLoopCoroutine);
            if (resumeMusicCoroutine != null) StopCoroutine(resumeMusicCoroutine);

            currentSceneMusic = newClip; // Save the scene music

            audioSource.clip = newClip;
            audioSource.Play();

            if (isCityScene)
            {
                audioSource.loop = false;
                cityLoopCoroutine = StartCoroutine(HandleCityLoop());
            }
            else
            {
                audioSource.loop = true;
            }
        }
    }

    private IEnumerator HandleCityLoop()
    {
        while (true)
        {
            yield return null;
            if (audioSource.time >= audioSource.clip.length - 0.1f)
            {
                audioSource.time = cityLoopStartTime;
                audioSource.Play();
            }
        }
    }

    // Plays a temporary music clip, pausing the current scene music and resuming it after.
    public void PlayTemporaryMusic(AudioClip tempClip, bool waitForCompletion = true)
    {
        if (tempClip == null) return;
        if (audioSource.clip == tempClip) return; // Prevent restarting the same track

        if (cityLoopCoroutine != null) StopCoroutine(cityLoopCoroutine);
        if (resumeMusicCoroutine != null) StopCoroutine(resumeMusicCoroutine);

        StartCoroutine(SwitchToTemporaryMusic(tempClip, waitForCompletion));
    }

    private IEnumerator SwitchToTemporaryMusic(AudioClip tempClip, bool waitForCompletion)
    {
        // Pause scene music and play temporary track
        float savedTime = audioSource.time;
        audioSource.Stop();
        audioSource.clip = tempClip;
        audioSource.loop = false;
        audioSource.Play();

        // Wait for temp music to finish, if required
        if (waitForCompletion)
        {
            yield return new WaitWhile(() => audioSource.isPlaying);
            ResumeSceneMusic(savedTime);
        }
    }

    /// Resumes the paused scene music after playing temporary music.
    public void ResumeSceneMusic(float resumeTime = 0f)
    {
        if (currentSceneMusic == null) return;

        audioSource.clip = currentSceneMusic;
        audioSource.time = resumeTime;
        audioSource.Play();

        if (isCityScene)
        {
            audioSource.loop = false;
            cityLoopCoroutine = StartCoroutine(HandleCityLoop());
        }
        else
        {
            audioSource.loop = true;
        }
    }
}
