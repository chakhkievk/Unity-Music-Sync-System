using UnityEngine;

public class MusicManager : AudioManager
{
    [Header("Music Settings")]
    
    [SerializeField] private bool _loopTrack = true;
    [SerializeField] private float trackTime = 0f;

   



    private void Start()
    {
        SetTime(trackTime);
        PlayTrack();
    }

    public void SetTime(float trackTime)
    {
        audioSource.time = trackTime;
    }



    public void PlayTrack()
    {
        audioSource.loop = _loopTrack;
        Play();
    }

    public void PauseTrack()
    {
        Pause();
    }

    public void ResumeTrack()
    {
        Resume();
    }

    public void StopTrack()
    {
        Stop();
    }

    public void RestartTrack()
    {
        StopTrack();
        PlayTrack();
    }

    public bool IsLooping()
    {
        return _loopTrack;
    }
}