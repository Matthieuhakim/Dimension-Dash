using UnityEngine;

// Moves to a per-dimension offset (and tints) on each switch, e.g. the eclipse disc sliding over the sun.
// Offsets are relative to the starting local position. Purely visual.
public class DimensionSlide : DimensionListener
{
    [SerializeField]
    private Vector3 realityOffset;
    [SerializeField]
    private Vector3 shadowOffset;
    [SerializeField]
    private float slideTime = 0.35f;
    [SerializeField]
    private Color realityColor = Color.white;
    [SerializeField]
    private Color shadowColor = Color.white;

    private Vector3 basePosition;
    private Vector3 slideFrom;
    private Vector3 slideTo;
    private float slideProgress = 1f;

    protected override void Awake()
    {
        basePosition = transform.localPosition;
        base.Awake();
    }

    protected override void OnDimensionChanged(Dimension dimension)
    {
        bool reality = dimension == Dimension.Reality;
        Tint = reality ? realityColor : shadowColor;

        Vector3 target = basePosition + (reality ? realityOffset : shadowOffset);
        if (IsApplyingInitialState || slideTime <= 0f)
        {
            transform.localPosition = target;
            slideProgress = 1f;
            return;
        }
        slideFrom = transform.localPosition;
        slideTo = target;
        slideProgress = 0f;
    }

    protected override void Update()
    {
        base.Update();
        if (slideProgress >= 1f) return;

        slideProgress = Mathf.Min(1f, slideProgress + Time.deltaTime / slideTime);
        transform.localPosition = Vector3.Lerp(slideFrom, slideTo, Mathf.SmoothStep(0f, 1f, slideProgress));
    }
}
