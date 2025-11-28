using UnityEngine;
using UnityEngine.Events;

public class BaseBeat : MonoBehaviour
{
    [SerializeField] protected MusicManager _musicManager;
    [SerializeField] protected OnsetData _onsetData;

    protected float _beatsPerMinute;
    protected float _beatInterval;

    protected virtual void Start()
    {
        _beatsPerMinute = _onsetData.beatsPerMinute;
        _beatInterval = 60f / _beatsPerMinute;
        Debug.Log($"[BaseBeat] BPM: {_beatsPerMinute}, Interval: {_beatInterval:F3}s");
    }

    public int GetCurrentBeat()
    {
        float currentTime = _musicManager.GetCurrentTime();
        return Mathf.FloorToInt(currentTime / _beatInterval);
    }

    public float GetBeatTime(int beatIndex)
    {
        return beatIndex * _beatInterval;
    }

    public float GetTimeUntilNextBeat()
    {
        int currentBeat = GetCurrentBeat();
        float nextBeatTime = GetBeatTime(currentBeat + 1);
        float currentTime = _musicManager.GetCurrentTime();
        return nextBeatTime - currentTime;
    }

    public float GetBeatInterval()
    {
        return _beatInterval;
    }

    public float GetBeatsPerMinute()
    {
        return _beatsPerMinute;
    }

    public void SetBeatsPerMinute(float newBpm)
    {
        _beatsPerMinute = newBpm;
        _beatInterval = 60f / _beatsPerMinute;
    }
}

[System.Serializable]
public class Intervals
{
    [SerializeField] public float _steps = 1.0f;
    [SerializeField] public UnityEvent _trigger;
    private int _lastInterval = -1;

    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * _steps);
    }

    public void CheckForNewInterval(float currentTime, float intervalLength)
    {
        int currentInterval = Mathf.FloorToInt(currentTime / intervalLength);

        if (currentInterval != _lastInterval)
        {
            _lastInterval = currentInterval;
            _trigger?.Invoke();
        }
    }
}