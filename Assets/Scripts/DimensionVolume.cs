using UnityEngine;
using UnityEngine.Rendering;

// Blends a post-processing Volume in or out with the dimension (e.g. night-only bloom).
[RequireComponent(typeof(Volume))]
public class DimensionVolume : DimensionListener
{
    [SerializeField, Range(0f, 1f)]
    private float realityWeight;
    [SerializeField, Range(0f, 1f)]
    private float shadowWeight = 1f;
    [SerializeField]
    private float blendTime = 0.25f;

    private Volume volume;
    private float blendFrom;
    private float blendTo;
    private float blendProgress = 1f;

    protected override void Awake()
    {
        volume = GetComponent<Volume>();
        base.Awake();
    }

    protected override void OnDimensionChanged(Dimension dimension)
    {
        float target = dimension == Dimension.Reality ? realityWeight : shadowWeight;
        if (IsApplyingInitialState || blendTime <= 0f)
        {
            volume.weight = target;
            blendProgress = 1f;
            return;
        }
        blendFrom = volume.weight;
        blendTo = target;
        blendProgress = 0f;
    }

    protected override void Update()
    {
        base.Update();
        if (blendProgress >= 1f) return;

        blendProgress = Mathf.Min(1f, blendProgress + Time.deltaTime / blendTime);
        volume.weight = Mathf.Lerp(blendFrom, blendTo, blendProgress);
    }
}
