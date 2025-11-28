using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AudioAnalyzer : BaseBeat
{
    [Header("Analysis Settings")]
    public bool muteAudio = true;


    // FFT parameters 
    private int _fftSize = 1024;
    private float[] _spectrum;
    private float[] _previousSpectrum;

    private List<float> _detectedOnsets = new List<float>();
    private float _lastOnsetTime = -1f;
    private bool _analysisCompleted = false;

    protected override void Start()
    {
#if UNITY_EDITOR
        string dataPath = EditorPrefs.GetString("OnsetDataPath", "");
        if (string.IsNullOrEmpty(dataPath))
        {
            Debug.LogError("[AudioAnalyzer] OnsetData path is null");
            return;
        }

        OnsetData targetData = AssetDatabase.LoadAssetAtPath<OnsetData>(dataPath);
        if (targetData == null)
        {
            Debug.LogError("[AudioAnalyzer] Failed to load OnsetData");
            return;
        }

        _onsetData = targetData;
#endif

        base.Start();

        
        _musicManager.audioSource.volume = muteAudio ? 0f : 1f;

        _spectrum = new float[_fftSize];
        _previousSpectrum = new float[_fftSize];

        _musicManager.PlayTrack();

        Debug.Log($"[AudioAnalyzer] Started analysis for: {_onsetData.audioClip.name}");
        Debug.Log($"[AudioAnalyzer] Track length: {_onsetData.audioClip.length:F2}s, Muted: {muteAudio}");
    }

    void Update()
    {
        if (_analysisCompleted)
            return;

        if (!_musicManager.audioSource.isPlaying)
        {
            return;
        }

        float currentTime = _musicManager.GetCurrentTime();

        
        if (currentTime >= _onsetData.audioClip.length - 0.1f)
        {
            SaveResults();
            return;
        }

        _musicManager.audioSource.GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
        float flux = CalculateSpectralFlux(_spectrum, _previousSpectrum);

        if (flux > _onsetData.fluxThreshold)
        {
            if (currentTime - _lastOnsetTime > _onsetData.minInterval)
            {
                float nearestBeat = FindNearestBeat(currentTime, _onsetData.beatsPerMinute);
                float distance = Mathf.Abs(currentTime - nearestBeat);
                if (distance < _onsetData.beatTolerance)
                {
                    _detectedOnsets.Add(nearestBeat);
                    _lastOnsetTime = currentTime;
                    Debug.Log($"[AudioAnalyzer] Onset detected at {nearestBeat:F3}s (flux: {flux:F2})");
                }
            }
        }

        System.Array.Copy(_spectrum, _previousSpectrum, _fftSize);

        
        if (!_musicManager.audioSource.isPlaying && currentTime > 0.1f)
        {
            SaveResults();
        }
    }

    private float CalculateSpectralFlux(float[] currentSpectrum, float[] previousSpectrum)
    {
        float flux = 0f;
        for (int i = _onsetData.lowFreqBand; i < _onsetData.highFreqBand; i++)
        {
            float difference = currentSpectrum[i] - previousSpectrum[i];
            if (difference > 0f)
            {
                flux = flux + difference;
            }
        }
        return flux;
    }

    private float FindNearestBeat(float currentTime, float bpm)
    {
        int beatIndex = Mathf.RoundToInt(currentTime / _beatInterval);
        float beatTime = GetBeatTime(beatIndex);
        return beatTime;
    }

    private void SaveResults()
    {
        if (_analysisCompleted)
            return;

        _analysisCompleted = true;

#if UNITY_EDITOR
        Debug.Log($"[AudioAnalyzer] Saving {_detectedOnsets.Count} onsets...");

        
        _detectedOnsets.Sort();

        
        _onsetData.onsetTimes.Clear();
        _onsetData.onsetTimes.AddRange(_detectedOnsets);

        
        EditorUtility.SetDirty(_onsetData);
        AssetDatabase.SaveAssets();

        Debug.Log($"[AudioAnalyzer] Analysis complete! Found {_detectedOnsets.Count} onsets");
        Debug.Log("[AudioAnalyzer] Results saved to OnsetData asset");
        Debug.Log("[AudioAnalyzer] *** MANUALLY EXIT PLAY MODE NOW! ***");

        
        if (_musicManager != null && _musicManager.audioSource != null)
        {
            _musicManager.audioSource.Stop();
        }


#endif
    }
}