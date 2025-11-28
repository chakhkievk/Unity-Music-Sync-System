using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "OnsetData", menuName = "AudioSync/Onset Data")]
public class OnsetData : ScriptableObject
{
    [Header("Track info")]
    public AudioClip audioClip;
    public float beatsPerMinute;

    [Header("Analysis Settings")]
    public int windowSize = 512;
    public int hopSize = 256;
    public float fluxThreshold = 0.5f;
    public float minInterval = 0.1f;
    public float beatTolerance = 0.05f;
    public int lowFreqBand = 0;
    public int highFreqBand = 64;

    [Header("Analysis Results")]
    public List<float> onsetTimes = new List<float>();

    public float GetNextOnsetTime(float currentTime)
    {
        if (onsetTimes.Count == 0)
        {
            return -1;
        }

        for (int i = 0; i < onsetTimes.Count; i++)
        {
            if (onsetTimes[i] > currentTime)
            {
                return onsetTimes[i];
            }
        }

        return -1;
    }

    public int GetNextOnsetIndex(float currentTime)
    {
        if (onsetTimes.Count == 0)
        {
            return -1;
        }

        for (int i = 0; i < onsetTimes.Count; i++)
        {
            if (onsetTimes[i] > currentTime)
            {
                return i;
            }
        }

        return -1;
    }

}