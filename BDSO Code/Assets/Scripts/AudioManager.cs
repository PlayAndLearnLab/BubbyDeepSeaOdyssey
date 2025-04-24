using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("---------- Audio Sources ----------")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("---------- Audio Clips ----------")]
    public AudioClip lidarBackground;
    public AudioClip sonarBackground;
    public AudioClip cameraBackground;
    public AudioClip homecomingBackground;
    public AudioClip underwaterAmbiance;
    public AudioClip lidarActivation;
    public AudioClip sonarActivation;
    public AudioClip cameraActivation;
    public AudioClip hitObstacle;
    public AudioClip goalReached;
    public AudioClip energyLow;

    private void Start()
    {
        string name = SceneManager.GetActiveScene().name;
        switch (name)
        {
            case "RadarLevel":
                musicSource.clip = lidarBackground;
                musicSource.volume = 1.0f;
                break;
            case "SonarLevel":
                musicSource.clip = sonarBackground;
                musicSource.volume = 1.0f;
                break;
            case "CameraLevel":
                musicSource.clip = cameraBackground;
                musicSource.volume = 0.55f;
                break;
                break;
            case "MainMenu":
                musicSource.clip = sonarBackground;
                musicSource.volume = 1.0f;
                break;
            default:
                musicSource.volume = 1.0f;
                break;
        }
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip, bool interrupt = false, float volumeScale = 1.0F)
    {
        if (!sfxSource.isPlaying)
        {
            sfxSource.PlayOneShot(clip, volumeScale);
        }
        else
        {
            if (interrupt)
            {
                sfxSource.Stop();
                sfxSource.PlayOneShot(clip, volumeScale);
            }
        }
    }

    public void StopSFX()
    {
        if (sfxSource.isPlaying)
        {
            sfxSource.Stop();
        }
    }

}
