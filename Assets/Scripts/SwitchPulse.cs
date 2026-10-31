using UnityEngine;

// Plays a burst (e.g. an expanding ring) when the dimension switches, tinted with the new dimension's colour.
// Ignores the initial dimension applied on enable.
[RequireComponent(typeof(ParticleSystem))]
public class SwitchPulse : DimensionListener
{
    [SerializeField]
    private Color realityColor = Color.white;
    [SerializeField]
    private Color shadowColor = Color.white;

    private ParticleSystem burst;
    private bool initialised;

    protected override void Awake()
    {
        base.Awake();
        burst = GetComponent<ParticleSystem>();
    }

    protected override void OnDimensionChanged(Dimension dimension)
    {
        Tint = dimension == Dimension.Reality ? realityColor : shadowColor;

        if (!initialised)
        {
            initialised = true;
            return;
        }
        burst.Play();
    }
}
