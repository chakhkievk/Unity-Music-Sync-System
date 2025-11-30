# Beat-Sync-For-Unity

An Unity asset for automatically synchronizing music to gameplay through event triggers and dynamic animation speed control. This system analyzes audio tracks to detect musical onsets (beats, hits, impacts) and provides tools to sync gameplay events and animations perfectly with the music.

## Features

- **Onset Detection**: Uses FFT and spectral flux analysis to detect high-energy musical events in audio tracks
- **BPM-Based Event System**: Trigger Unity events at precise musical intervals (beats, half-beats, quarter-beats, etc.)
- **Dynamic Animation Sync**: Automatically adjusts animation speed to align animation events with music onsets
- **ScriptableObject Architecture**: Easy to manage multiple tracks with persistent onset data
- **Real-time Sync**: Maintains synchronization during playback with configurable smoothing

## System Architecture

### Core Components

#### 1. Audio Analysis (AudioAnalyzer.cs)

Analyzes audio tracks to detect onsets:

- Uses FFT (Fast Fourier Transform) with configurable window size
- Calculates spectral flux to identify sudden energy changes
- Aligns detected onsets to nearest beat based on BPM
- Saves results to OnsetData ScriptableObject

**Key Parameters:**

- `fluxThreshold`: Sensitivity for onset detection (higher = fewer onsets)
- `minInterval`: Minimum time between detected onsets
- `beatTolerance`: How close an onset must be to a beat to be registered
- `lowFreqBand` / `highFreqBand`: Frequency range to analyze (0-1024)

#### 2. Event Management

**OnsetEventManager (OnsetEventManager.cs)**

- Triggers `OnOnsetHit` Unity event when music reaches detected onset times
- Provides `BeatData` with time, strength, and index
- Handles track looping automatically
- Use for triggering gameplay events synchronized to music

**BeatsPerMinuteEventManager (BeatsPerMinuteEventManager.cs)**

- Triggers events at regular BPM intervals
- Supports multiple interval configurations (e.g., every beat, every 2 beats, etc.)
- Independent of onset detection
- Use for metronome-like events or visual feedback

#### 3. Animation Sync (AnimationSyncController.cs)

Dynamically adjusts animation playback speed to sync with music:

- Monitors animation clips for `OnbeatHit` animation events
- Calculates required speed to align next animation event with next music onset
- Supports smooth speed transitions or instant changes
- Configurable speed limits to prevent extreme values

**Key Settings:**

- `smoothAnimationSpeedChange`: Smoothly lerp speed changes vs instant
- `speedLimitMin` / `speedLimitMax`: Clamp animation speed (default: 0.5x - 2x)

#### 4. Data Management

**OnsetData (OnsetData.cs)** ScriptableObject storing track analysis results:

- Audio clip reference
- BPM (Beats Per Minute)
- Analysis parameters
- List of detected onset times

**OnsetDataEditor (OnsetDataEditor.cs)** Custom inspector with "Analyze Track" button to trigger analysis workflow

## Getting Started

### 1. Setup Music Manager

Add `MusicManager` component to a GameObject:

- Assign `AudioSource` component
- Configure loop settings
- Set initial track time if needed

### 2. Create OnsetData Asset

1. Right-click in Project window
2. Create > AudioSync > Onset Data
3. Configure settings:
   - Assign your AudioClip
   - Set BPM (beats per minute)
   - Adjust analysis parameters:
     - `fluxThreshold`: Start with 0.5, adjust based on results
     - `minInterval`: Minimum time between onsets (default 0.1s)
     - `beatTolerance`: How close to beat to snap (default 0.05s)
     - `lowFreqBand` / `highFreqBand`: Frequency range to analyze

### 3. Analyze Track

**IMPORTANT:** Follow these steps carefully for proper analysis:

1. Select your OnsetData asset in the Project window
2. Click "Analyze Track" button in the Inspector (this prepares the analysis)
3. Open the `AudioAnalysisScene` (the dedicated scene for analysis)
4. In the AudioAnalysisScene, find the AudioAnalyzer GameObject
5. Assign your OnsetData asset to the AudioAnalyzer component's OnsetData field
6. Verify analysis settings in the assigned OnsetData asset
7. Press Play in Unity Editor
8. Wait for the entire track to finish playing (watch Console for progress)
9. When complete, Console will show: "Analysis complete! Found X onsets"
10. Console will prompt: "MANUALLY EXIT PLAY MODE NOW!"
11. Exit Play mode
12. Your OnsetData asset now contains all detected onset times

### 4. Use Onset Events (Method A)

Add `OnsetEventManager` to your scene:

- Assign MusicManager reference
- Assign OnsetData asset
- Subscribe to `OnOnsetHit` event in Inspector or code

**Example subscription:**
```csharp
public class GameplayController : MonoBehaviour
{
    [SerializeField] private OnsetEventManager onsetManager;

    void Start()
    {
        onsetManager.OnOnsetHit.AddListener(OnMusicHit);
    }

    void OnMusicHit(BeatData beatData)
    {
        Debug.Log($"Music hit at {beatData.time}s!");
        // Trigger gameplay event, spawn enemy, flash screen, etc.
    }
}
```

### 5. Sync Animations (Method B)

Add `AnimationSyncController` to character GameObject:

- Assign Animator component
- Assign MusicManager reference
- Assign OnsetData asset
- Configure speed limits

Add `OnbeatHit` animation events to your animation clips:

1. Select your character with an animator
2. Open Animation window
3. Select the animation you want to sync with the music
3. Add Animation Event `OnbeatHit` at desired frames

The controller will automatically adjust animation speed so these events align with music onsets.

### 6. BPM-Based Events (Method C)

Add `BeatsPerMinuteEventManager` for metronome-like events:

- Assign MusicManager reference
- Assign OnsetData asset
- Add Intervals in Inspector:
  - **Steps**: 1.0 = every beat, 0.5 = every half beat, 2.0 = every 2 beats
  - **Trigger**: UnityEvent to invoke at each interval

## Workflow Summary
```
1. Create OnsetData ScriptableObject
   ↓
2. Assign AudioClip, set BPM, and configure analysis settings in ScriptableObject
   ↓
3. Click "Analyze Track" button in Inspector
   ↓
4. Open AudioAnalysisScene
   ↓
5. Assign your OnsetData asset to AudioAnalyzer component in the scene
   ↓
6. Press Play and wait for entire track to finish
   ↓
7. Analysis runs (FFT + spectral flux detection throughout playback)
   ↓
8. Onset times automatically saved to OnsetData asset
   ↓
9. Exit Play mode when prompted
   ↓
10. Use OnsetEventManager OR AnimationSyncController OR BeatsPerMinuteEventManager
    ↓
11. Events/animations sync automatically during gameplay
```

## Advanced Configuration

### Fine-Tuning Onset Detection

**Too Many Onsets:**

- Increase `fluxThreshold` (0.5 → 1.0+)
- Increase `minInterval` (0.1 → 0.2+)
- Narrow frequency band (reduce `highFreqBand`)

**Too Few Onsets:**

- Decrease `fluxThreshold` (0.5 → 0.2-0.3)
- Decrease `beatTolerance` (0.05 → 0.03)
- Widen frequency band (increase `highFreqBand`)

**Missing Bass Hits:**

- Lower `lowFreqBand` (0 → 0)
- Increase `highFreqBand` to include low-mid range

**Missing High Percussion:**

- Increase `lowFreqBand` to focus on higher frequencies

### Animation Sync Best Practices

- **Animation Event Placement**: Place `OnbeatHit` events at key animation moments (punches, kicks, impacts)
- **Speed Limits**: Keep within 0.5x - 2.0x to avoid visual glitches
- **Smooth Transitions**: Enable `smoothAnimationSpeedChange` for fluid speed changes
- **Loop Handling**: System automatically resets when animations loop

## Script Reference

### MusicManager
```csharp
PlayTrack()           // Start playing music
PauseTrack()          // Pause music
ResumeTrack()         // Resume from pause
StopTrack()           // Stop music
RestartTrack()        // Stop and replay from beginning
SetTime(float time)   // Jump to specific time
IsLooping()          // Check if track loops
GetCurrentTime()      // Get current playback time
```

### OnsetEventManager
```csharp
OnOnsetHit           // UnityEvent<BeatData> triggered on each onset
```

### AnimationSyncController
```csharp
GetCurrentClipTime()        // Current time in animation clip
GetNextAnimEventTime()      // Time until next OnbeatHit event
OnbeatHit()                 // Called by animation events (automatic)
```

### BaseBeat (inherited by BPM and Onset managers)
```csharp
GetCurrentBeat()            // Current beat index
GetBeatTime(int index)      // Get time of specific beat
GetTimeUntilNextBeat()      // Time until next beat
GetBeatInterval()           // Seconds per beat (60/BPM)
SetBeatsPerMinute(float)    // Change BPM dynamically
```

### OnsetData
```csharp
GetNextOnsetTime(float currentTime)   // Get next onset after given time
GetNextOnsetIndex(float currentTime)  // Get index of next onset
```

## Example Use Cases

### Rhythm Game

Use `OnsetEventManager` to spawn obstacles on music hits

### Beat 'Em Up / Action Game

Use `AnimationSyncController` to sync attack animations with music for satisfying combat

### Visual Effects

Use `BeatsPerMinuteEventManager` to pulse lights or environment on beats

## Technical Details

- **FFT Window**: 1024 samples (configurable via `_fftSize` in AudioAnalyzer.cs:14)
- **FFT Window Function**: BlackmanHarris for better frequency resolution
- **Spectral Flux**: Sum of positive differences between consecutive spectra
- **Beat Quantization**: Detected onsets are snapped to nearest beat based on BPM
- **Frequency Range**: Default 0-64 bins (approximately 0-1378 Hz at 44.1kHz sample rate)

## Tips and Troubleshooting

**Problem:** Animation sync feels laggy or imprecise  
**Solution:** Disable `smoothAnimationSpeedChange` for instant speed adjustments

**Problem:** Animations speed up/slow down too much  
**Solution:** Tighten `speedLimitMin`/`speedLimitMax` range (e.g., 0.8x - 1.2x)

**Problem:** Onsets detected at wrong times  
**Solution:** Verify BPM is correct. Use online BPM detector or tap tempo

**Problem:** Analysis finds no onsets  
**Solution:** Check audio import settings (load type should allow GetData)  
**Solution:** Ensure track has sufficient dynamic range  
**Solution:** Lower `fluxThreshold` value

**Problem:** Events trigger multiple times  
**Solution:** Increase `minInterval` in OnsetData

**Problem:** Forgot to assign OnsetData in AudioAnalysisScene  
**Solution:** The AudioAnalyzer will log an error. Make sure to assign your OnsetData asset before pressing Play

**Problem:** Analysis stopped early  
**Solution:** Ensure you wait for the ENTIRE track to finish. Don't exit Play mode early or onset data will be incomplete

## File Structure
```
Assets/
├── Scripts/
│   ├── Audio/
│   │   ├── AudioManager.cs              # Base audio control
│   │   ├── MusicManager.cs              # Music playback manager
│   │   └── BeatAndBpm/
│   │       ├── BaseBeat.cs              # BPM calculation base class
│   │       ├── AudioAnalyzer.cs         # Onset detection analysis
│   │       ├── OnsetEventManager.cs     # Onset-based event system
│   │       └── BeatsPerMinuteEventManager.cs  # BPM-based event system
│   ├── Animations/
│   │   └── AnimationSyncController.cs   # Dynamic animation speed sync
│   ├── ScriptableObjects/
│   │   └── OnsetData.cs                 # Onset data container
│   └── Editor/
│       └── OnsetDataEditor.cs           # Custom inspector for OnsetData
├── Scenes/
│   └── AudioAnalysisScene               # Dedicated scene for analyzing tracks
```

## Requirements

- Audio clips must be imported with "Load Type" compatible with GetData()

## License

MIT License - see LICENSE file.
