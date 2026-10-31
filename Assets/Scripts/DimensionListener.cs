using UnityEngine;
using UnityEngine.Tilemaps;

// Base for anything that reacts to the level's dimension switches.
// Tints a SpriteRenderer (on this object or its children), a Tilemap, a ParticleSystem or a Camera's background.
// Tint changes crossfade over fadeTime (visual only: subclasses apply gameplay changes instantly).
public abstract class DimensionListener : MonoBehaviour
{
    [Tooltip("Seconds a tint change takes to blend in. 0 = instant.")]
    [SerializeField]
    private float fadeTime = 0.15f;

    private SpriteRenderer spriteTarget;
    private Tilemap tilemapTarget;
    private ParticleSystem particleTarget;
    private Camera cameraTarget;

    private Color fadeFrom;
    private Color fadeTo;
    private float fadeProgress = 1f;
    private bool snapNextTint;

    protected Color Tint
    {
        get => fadeProgress < 1f ? fadeTo : CurrentTint;
        set
        {
            if (snapNextTint || fadeTime <= 0f || !Application.isPlaying)
            {
                fadeProgress = 1f;
                CurrentTint = value;
                return;
            }
            fadeFrom = CurrentTint;
            fadeTo = value;
            fadeProgress = 0f;
        }
    }

    private Color CurrentTint
    {
        get
        {
            if (spriteTarget != null) return spriteTarget.color;
            if (tilemapTarget != null) return tilemapTarget.color;
            if (particleTarget != null) return particleTarget.main.startColor.color;
            if (cameraTarget != null) return cameraTarget.backgroundColor;
            return Color.white;
        }
        set
        {
            if (spriteTarget != null) spriteTarget.color = value;
            else if (tilemapTarget != null) tilemapTarget.color = value;
            else if (particleTarget != null)
            {
                ParticleSystem.MainModule main = particleTarget.main;
                main.startColor = value;
            }
            else if (cameraTarget != null) cameraTarget.backgroundColor = value;
        }
    }

    protected virtual void Awake()
    {
        FindTintTarget();
    }

    protected virtual void OnEnable()
    {
        LevelManager.DimensionChanged += OnDimensionChanged;

        if (LevelManager.Instance != null)
        {
            // The initial state appears at once; only switches fade.
            snapNextTint = true;
            OnDimensionChanged(LevelManager.Instance.CurrentDimension);
            snapNextTint = false;
        }
    }

    protected virtual void OnDisable()
    {
        LevelManager.DimensionChanged -= OnDimensionChanged;
    }

    protected virtual void Update()
    {
        if (fadeProgress >= 1f) return;

        fadeProgress = Mathf.Min(1f, fadeProgress + Time.deltaTime / fadeTime);
        float t = 1f - (1f - fadeProgress) * (1f - fadeProgress);
        CurrentTint = Color.LerpUnclamped(fadeFrom, fadeTo, t);
    }

    // True while the initial dimension is being applied on enable: subclasses should snap, not animate.
    protected bool IsApplyingInitialState => snapNextTint;

    protected abstract void OnDimensionChanged(Dimension dimension);

    protected void FindTintTarget()
    {
        if (TryGetComponent(out tilemapTarget) || TryGetComponent(out particleTarget) || TryGetComponent(out cameraTarget)) return;
        spriteTarget = GetComponentInChildren<SpriteRenderer>();
    }
}
