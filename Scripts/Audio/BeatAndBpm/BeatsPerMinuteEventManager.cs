using UnityEngine;
using System.Collections.Generic;

public class BeatsPerMinuteEventManager : BaseBeat
{
    [Header("Intervals System")]
    [SerializeField] private List<Intervals> _intervals = new List<Intervals>();

    private void Update()
    {
        float currentTime = _musicManager.GetCurrentTime();

        foreach (Intervals interval in _intervals)
        {
            float intervalLength = interval.GetIntervalLength(_beatsPerMinute);
            interval.CheckForNewInterval(currentTime, intervalLength);
        }
    }
}