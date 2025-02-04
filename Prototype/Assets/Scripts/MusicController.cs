/// <summary>
/// Nestoras Angelopoulos 2025
/// 
/// Script for a singleton GameObject that manages the music playlist and track changes.
/// </summary>
using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance;

    [SerializeField] AudioMixerGroup musicGroup;
    AudioSource audioSource;
    AudioSource secondaryAudioSource;

    [SerializeField] AudioClip[] tracks;
    int currentTrack;

    [Tooltip("When enabled, loops the same track. When disabled, loops through the whole playlist.")]
    public bool repeat;
    [Tooltip("When enabled, picks a random track when the current one ends, avoiding replays.")]
    public bool shuffle;
    public bool paused;
    bool unpaused = true;

    bool changingTracks;
    float transitionDuration;
    double transitionTime;

    private void Awake()
    {
        // Make sure this is the only MusicController in the scene.
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create an AudioSource for the music.
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = musicGroup;
        audioSource.playOnAwake = false;
        audioSource.clip = tracks[currentTrack];
    }

    void Update()
    {
        // Pause and Un-pause the current track.
        if (paused)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Pause();
                unpaused = true;
            }
            return;
        }
        else if (unpaused)
        {
            unpaused = false;
            audioSource.Play();
        }

        // Fade the old track out, and the new track in.
        if (changingTracks)
        {
            audioSource.volume -= 1 / transitionDuration * Time.deltaTime;
            secondaryAudioSource.volume += 1 / transitionDuration * Time.deltaTime;
        }

        // When a track finishes, play the next one, or repeat the same one.
        if (audioSource.isPlaying) return;
        else if (!repeat)
        {
            if (shuffle && tracks.Length > 1) // Shuffle.
            {
                int nextTrack;
                do nextTrack = Random.Range(0, tracks.Length);
                while (nextTrack == currentTrack);
                currentTrack = nextTrack;
            }
            else // Move to next in order.
            {
                currentTrack++;
                if (currentTrack >= tracks.Length) currentTrack = 0;
            }
        }

        audioSource.clip = tracks[currentTrack];
        audioSource.Play();
    }

    /// <summary>
    /// Fade from the current track to the track with the provided index.
    /// </summary>
    /// <param name="trackIndex">The index of the track to transition to.</param>
    /// <param name="timeToTransition">The time it'll take for the track to fade in.</param>
    public void ChangeTrack(int trackIndex, float timeToTransition = 0f)
    {
        if (changingTracks) TransitionError();
        else StartCoroutine(TransitionMusic(trackIndex, timeToTransition));
    }

    /// <summary>
    /// Fade from the current track to the provided track. When the new track finishes, the playlist will continue from where it left off.
    /// </summary>
    /// <param name="track">The AudioClip to shoehorn into the playlist.</param>
    /// <param name="timeToTransition">The time it'll take for the track to fade in.</param>
    /// <param name="loop">Loop the shoehorned track? Needs manual resetting after transition.</param>
    /// <param name="trackIndexToFollow">The index of the track to be played after the provided track finishes.</param>
    public void ChangeTrack(AudioClip track, float timeToTransition = 0f, bool loop = false, int trackIndexToFollow = -1)
    {
        if (changingTracks) TransitionError();
        else if (trackIndexToFollow == -1) StartCoroutine(TransitionMusic(currentTrack, timeToTransition, track));
        else StartCoroutine(TransitionMusic(trackIndexToFollow, timeToTransition, track, loop));
    }

    void TransitionError()
    {
        Debug.LogError($"A track transition is already taking place. Please wait at least {((int)((transitionTime + transitionDuration - Time.time) * 100f)) / 100f} seconds before starting a new transition.");
    }

    IEnumerator TransitionMusic(int trackIndex, float timeToTransition, AudioClip track = null, bool loop = false)
    {
        // Create a new AudioSource to play the new track.
        secondaryAudioSource = gameObject.AddComponent<AudioSource>();
        secondaryAudioSource.outputAudioMixerGroup = musicGroup;
        secondaryAudioSource.playOnAwake = false;
        secondaryAudioSource.volume = 0f;
        secondaryAudioSource.loop = loop;
        if (track == null) secondaryAudioSource.clip = tracks[trackIndex];
        else secondaryAudioSource.clip = track;
        secondaryAudioSource.Play();

        // Fade the old track out, and the new track in.
        changingTracks = true;
        transitionDuration = timeToTransition;
        transitionTime = Time.time;
        yield return new WaitForSeconds(timeToTransition);
        
        // Replace the audioSource so that the playlist can resume.
        changingTracks = false;
        Destroy(audioSource);
        audioSource = secondaryAudioSource;
        audioSource.volume = 1f;
        currentTrack = trackIndex;
    }
}