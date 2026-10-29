using UnityEngine;
using UnityEngine.Tilemaps;

// Base for anything that reacts to the level's dimension switches.
// Tints a SpriteRenderer (on this object or its children), a Tilemap, or a Camera's background.
public abstract class DimensionListener : MonoBehaviour
{
    private SpriteRenderer spriteTarget;
    private Tilemap tilemapTarget;
    private Camera cameraTarget;

    protected Color Tint
    {
        get
        {
            if (spriteTarget != null) return spriteTarget.color;
            if (tilemapTarget != null) return tilemapTarget.color;
            if (cameraTarget != null) return cameraTarget.backgroundColor;
            return Color.white;
        }
        set
        {
            if (spriteTarget != null) spriteTarget.color = value;
            else if (tilemapTarget != null) tilemapTarget.color = value;
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
            OnDimensionChanged(LevelManager.Instance.CurrentDimension);
        }
    }

    protected virtual void OnDisable()
    {
        LevelManager.DimensionChanged -= OnDimensionChanged;
    }

    protected abstract void OnDimensionChanged(Dimension dimension);

    protected void FindTintTarget()
    {
        if (TryGetComponent(out tilemapTarget) || TryGetComponent(out cameraTarget)) return;
        spriteTarget = GetComponentInChildren<SpriteRenderer>();
    }
}
