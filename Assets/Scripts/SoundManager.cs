using System;
using UnityEngine;

// Per-scene sound effects plus music that keeps playing across level restarts.
// Gameplay code calls SoundManager.Play(...); dimension switches are picked up from LevelManager.DimensionChanged.
public class SoundManager : MonoBehaviour
{
    public enum Sfx { Jump, Land, SwitchToReality, SwitchToShadow, Death, Finish, Click }

    [Serializable]
    private struct Effect
    {
        public Sfx id;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        [Tooltip("Random pitch variation, ± this amount.")]
        [Range(0f, 0.3f)]
        public float pitchJitter;
    }

    [SerializeField]
    private Effect[] effects;
    [SerializeField]
    private AudioClip music;
    [SerializeField, Range(0f, 1f)]
    private float musicVolume = 0.45f;

    private static SoundManager instance;
    // Survives scene reloads so a death doesn't restart the track.
    private static AudioSource musicSource;

    // A few voices so overlapping effects keep their own pitch.
    private readonly AudioSource[] voices = new AudioSource[4];
    private int nextVoice;

    private void Awake()
    {
        instance = this;
        for (int i = 0; i < voices.Length; i++)
        {
            voices[i] = gameObject.AddComponent<AudioSource>();
            voices[i].playOnAwake = false;
        }
        StartMusic();
    }

    private void OnEnable() => LevelManager.DimensionChanged += OnDimensionChanged;

    private void OnDisable() => LevelManager.DimensionChanged -= OnDimensionChanged;

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }

    public static void Play(Sfx id)
    {
        if (instance != null) instance.PlayEffect(id);
    }

    private void OnDimensionChanged(Dimension dimension)
    {
        PlayEffect(dimension == Dimension.Reality ? Sfx.SwitchToReality : Sfx.SwitchToShadow);
    }

    private void PlayEffect(Sfx id)
    {
        foreach (Effect effect in effects)
        {
            if (effect.id != id || effect.clip == null) continue;

            AudioSource voice = voices[nextVoice];
            nextVoice = (nextVoice + 1) % voices.Length;
            voice.pitch = 1f + UnityEngine.Random.Range(-effect.pitchJitter, effect.pitchJitter);
            voice.clip = effect.clip;
            voice.volume = effect.volume;
            voice.Play();
            return;
        }
    }

    private void StartMusic()
    {
        if (music == null) return;

        if (musicSource == null)
        {
            var musicObject = new GameObject("Music");
            DontDestroyOnLoad(musicObject);
            musicSource = musicObject.AddComponent<AudioSource>();
            musicSource.loop = true;
        }

        musicSource.volume = musicVolume;
        if (musicSource.clip != music || !musicSource.isPlaying)
        {
            musicSource.clip = music;
            musicSource.Play();
        }
    }
}
