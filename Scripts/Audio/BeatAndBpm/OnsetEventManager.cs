using UnityEngine;
using UnityEngine.Events;

public class OnsetEventManager : BaseBeat
{
    private int _nextOnsetIndex = 0;

    public UnityEvent<BeatData> OnOnsetHit;

    private void Update()
    {
        if (_nextOnsetIndex < 0)
            return;

        float currentTrackTime = _musicManager.GetCurrentTime();

        
        if (currentTrackTime < 1.0f && _nextOnsetIndex > 0)
        {
            _nextOnsetIndex = 0; 
            Debug.Log("[OnsetManager] Track looped, reset onset index to 0");
        }

        if (_nextOnsetIndex >= _onsetData.onsetTimes.Count)
        {
            if (_musicManager.IsLooping())
            {
                _nextOnsetIndex = 0;
            }
            else
            {
                _nextOnsetIndex = -1;
                return;
            }
        }

        float nextOnsetTime = _onsetData.onsetTimes[_nextOnsetIndex];

        if (currentTrackTime >= nextOnsetTime && _nextOnsetIndex < _onsetData.onsetTimes.Count - 1)
        {
            BeatData beatData = new BeatData();
            beatData.time = nextOnsetTime;
            beatData.strength = 1.0f;
            beatData.index = _nextOnsetIndex;
            OnOnsetHit?.Invoke(beatData);
            _nextOnsetIndex++;
        }
    }
}


public class BeatData
{
    public float time;
    public float strength;
    public int index;
}