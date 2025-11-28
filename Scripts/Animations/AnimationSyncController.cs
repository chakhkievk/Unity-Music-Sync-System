using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimationSyncController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;
    [SerializeField] private OnsetData _onsetData;
    [SerializeField] private MusicManager _musicManager;

    [Header("Settings")]
    [SerializeField] private bool _smoothAnimationSpeedChange = true;
    [SerializeField] private float _speedLimitMin = 0.5f;
    [SerializeField] private float _speedLimitMax = 2f;

    
    private int _eventIndex = 0;
    private List<float> _currentEventTimes = new List<float>();
    private float _nextEventTime;

    private AnimationClip _lastClip;

    

    void Start()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    void Update()
    {
        AnimatorClipInfo[] clipInfos = _animator.GetCurrentAnimatorClipInfo(0);
        if (clipInfos.Length > 0 && clipInfos[0].clip != _lastClip)
        {
            _lastClip = clipInfos[0].clip;
            UpdateEventTimes();
            _eventIndex = 0;
        }

        _nextEventTime = GetNextAnimEventTime();

        
       

        if (_nextEventTime != -1f)
        {
            ChangeClipSpeedToMatchTheBeat();
        }
        
    }

    void UpdateEventTimes()
    {
        if (_lastClip == null) return;

        // Get all OnbeatHit event times
        _currentEventTimes.Clear();
        foreach (AnimationEvent evt in _lastClip.events)
        {
            if (evt.functionName == "OnbeatHit")
            {
                _currentEventTimes.Add(evt.time);
            }
        }
        _currentEventTimes = _currentEventTimes.OrderBy(t => t).ToList();
    }

    // Unity calls this automatically through AnimationEvents!
    public void OnbeatHit()
    {
        

        
        if (_eventIndex + 1 >= _currentEventTimes.Count)
        {
            _eventIndex = 0;
            if (_smoothAnimationSpeedChange)
            {
                // Smooth speed changes to avoid jitter
                _animator.speed = Mathf.Lerp(_animator.speed, 1f, Time.deltaTime * 5f);
            }
            else
            {
                _animator.speed = 1f;
            }
        }
        else
        {
            _eventIndex++;
        }
    }

    public float GetCurrentClipTime()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (_lastClip == null) return 0f;

        float normalizedTime = stateInfo.normalizedTime % 1f;
        return normalizedTime * _lastClip.length;
    }

    public float GetNextAnimEventTime()
    {
        if (_currentEventTimes.Count == 0 || _eventIndex >= _currentEventTimes.Count)
            return -1f;

        float currentTime = GetCurrentClipTime();
        float nextEvent = _currentEventTimes[_eventIndex];

        
        if (nextEvent > currentTime)
        {
            return nextEvent - currentTime;
        }

        // If looping, calculate time to next loop's first event
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.loop && _lastClip != null)
        {
            return (_lastClip.length - currentTime) + _currentEventTimes[0];
        }

        return -1f;
    }

    public void ChangeClipSpeedToMatchTheBeat()
    {
        if (GetNextAnimEventTime() == -1f) return;

        float currentTrackTime = _musicManager.GetCurrentTime();
        float timeUntilAnimEvent = GetNextAnimEventTime();
        float timeUntilBeat = _onsetData.GetNextOnsetTime(currentTrackTime) - currentTrackTime;

        
        if (timeUntilBeat <= 0.01f) return;

        
        float desiredSpeed = timeUntilAnimEvent / timeUntilBeat;

        
        desiredSpeed = Mathf.Clamp(desiredSpeed, _speedLimitMin, _speedLimitMax);

        if (_smoothAnimationSpeedChange)
        {
            // Smooth speed changes to avoid jitter
            _animator.speed = Mathf.Lerp(_animator.speed, desiredSpeed, Time.deltaTime * 5f);
            
        }
        else
        {
            _animator.speed = desiredSpeed;
        }
        Debug.Log(_animator.speed);

    }
}