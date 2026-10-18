using UnityEditor.Timeline.Actions;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Instancia estática para el patrón Singleton
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips de Ejemplo")]
    public AudioClip backgroundMusic;
    public AudioClip backgroundMusicMenu;
    public AudioClip[] jumpSFX;
    public AudioClip shotSfx;
    public AudioClip golpeSfx;
    public AudioClip winSFX;
    public AudioClip powerupSFX;
    public AudioClip deadSFX;
    public AudioClip flagPointSFX;

    private void Awake()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No se destruye al cambiar de escena
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Reproducir música de fondo al iniciar si se asignó una
        /*         if (backgroundMusic != null)
                {
                    PlayMusic(backgroundMusic);
                } */
    }

    // Método para reproducir Música (Bucle activado por defecto)
    public void PlayMatchMusic()
    {
        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void PlayMenuMusic()
    {
        musicSource.clip = backgroundMusicMenu;
        musicSource.loop = true;
        musicSource.Play();
    }

    // Método para reproducir Efectos de Sonido (SFX)
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        // PlayOneShot permite reproducir sonidos simultáneos sin cortarse entre sí
        sfxSource.PlayOneShot(clip);
    }
    public void PlaySalto()
    {
        sfxSource.PlayOneShot(jumpSFX[Random.Range(0, jumpSFX.Length)]);
    }
    public void PlayGolpe()
    {
        sfxSource.PlayOneShot(golpeSfx);
    }
    public void PlayMuerte()
    {
        if (deadSFX == null) return;
        sfxSource.PlayOneShot(deadSFX);
    }
    public void PlayDisparo()
    {
        sfxSource.PlayOneShot(shotSfx);
    }
    public void PlayFlagPoint()
    {
        sfxSource.PlayOneShot(flagPointSFX);
    }
    public void PlayWin()
    {
        sfxSource.PlayOneShot(winSFX);

    }
    // Métodos opcionales para pausar/detener
    public void StopMusic() => musicSource.Stop();
}
