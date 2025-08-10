using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioClip backgroundMusicMenu;
    public AudioClip backgroundMusicPause;
    private AudioSource _audioSource;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _audioSource = gameObject.AddComponent<AudioSource>();
        _audioSource.loop = true;
        _audioSource.playOnAwake = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicBasedOnScene(scene.name);
    }

    private void PlayMusicBasedOnScene(string sceneName)
    {
        if (sceneName == "MainMenu")
        {
            PlayMusic(backgroundMusicMenu);
        }
        else if (sceneName == "Pause")
        {
            PlayMusic(backgroundMusicPause);
        }
        else
        {
            StopMusic();
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (_audioSource.clip == clip) return;

        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private void StopMusic()
    {
        _audioSource.Stop();
        _audioSource.clip = null;
    }
}
