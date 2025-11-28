using UnityEngine;


public class AudioManager : MonoBehaviour
{
    [SerializeField] public AudioSource audioSource;



    public void Play()
    {
        audioSource.Play();
    }

    public void Pause()
    {
        audioSource.Pause();
    }

    public void Resume()
    {
        audioSource.UnPause();
    }

    public void Stop()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
    }

    public void SetPitch(float pitch)
    {
        audioSource.pitch = pitch;
    }


    public float GetCurrentTime()
    {
        return audioSource.time;
    }
}
