using UnityEngine;
using UnityEngine.Serialization;

// An object that only exists (is solid) in one dimension, or in both.
// When absent it is ghosted: collision off, tint faded. Works on sprites and tilemaps.
public class DimensionObject : DimensionListener
{
    [FormerlySerializedAs("preferedDimension")]
    [SerializeField]
    private DimensionPresence presentIn = DimensionPresence.Both;

    [Header("Colour per presence")]
    [SerializeField]
    private Color realityColor = Color.white;
    [SerializeField]
    private Color shadowColor = Color.white;
    [SerializeField]
    private Color bothColor = Color.white;

    [SerializeField, Range(0f, 1f)]
    private float ghostAlpha = 0.4f;

    private Rigidbody2D body;
    private Collider2D[] colliders;

    public DimensionPresence PresentIn => presentIn;

    private Color PresenceColor => presentIn switch
    {
        DimensionPresence.Reality => realityColor,
        DimensionPresence.Shadow => shadowColor,
        _ => bothColor,
    };

    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
    }

    public bool IsPresentIn(Dimension dimension)
    {
        return presentIn == DimensionPresence.Both || (int)presentIn == (int)dimension;
    }

    protected override void OnDimensionChanged(Dimension dimension)
    {
        bool present = IsPresentIn(dimension);

        // A body (e.g. a tilemap's composite collider) is toggled as a whole: cheaper than
        // re-enabling colliders, which would rebuild the composite geometry.
        if (body != null)
        {
            body.simulated = present;
        }
        else
        {
            foreach (Collider2D objectCollider in colliders)
            {
                objectCollider.enabled = present;
            }
        }

        Color color = PresenceColor;
        color.a = present ? 1f : ghostAlpha;
        Tint = color;
    }

    // Preview the level's start (Reality) in the editor: absent objects show ghosted.
    private void OnValidate()
    {
        if (Application.isPlaying) return;
        FindTintTarget();
        Color color = PresenceColor;
        color.a = IsPresentIn(Dimension.Reality) ? 1f : ghostAlpha;
        Tint = color;
    }
}
