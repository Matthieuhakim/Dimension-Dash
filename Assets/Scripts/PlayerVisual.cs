using UnityEngine;

// The orb's look: squash & stretch on jump/land, a pulsing ring, blinking eyes, and a trail in the orb's colour.
// Purely visual; PlayerController tells it when the player jumps and lands.
public class PlayerVisual : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer core;
    [SerializeField]
    private SpriteRenderer ring;
    [SerializeField]
    private Transform eyes;
    [SerializeField]
    private TrailRenderer trail;

    [Header("Squash & stretch")]
    [Tooltip("Scale applied on take-off (x thinner, y taller).")]
    [SerializeField]
    private Vector2 jumpStretch = new Vector2(0.78f, 1.25f);
    [Tooltip("Scale applied on landing (x wider, y shorter).")]
    [SerializeField]
    private Vector2 landSquash = new Vector2(1.3f, 0.72f);
    [SerializeField]
    private float springStiffness = 260f;
    [SerializeField]
    private float springDamping = 14f;
    [Tooltip("Half the orb's height: squashing keeps its bottom on the ground.")]
    [SerializeField]
    private float radius = 0.6f;

    [Header("Life")]
    [SerializeField]
    private float ringPulseSpeed = 4f;
    [SerializeField]
    private float ringPulseAmount = 0.08f;
    [SerializeField]
    private Vector2 blinkInterval = new Vector2(1.5f, 4f);
    [SerializeField]
    private float blinkDuration = 0.12f;

    private Vector2 scale = Vector2.one;
    private Vector2 scaleVelocity;
    private float nextBlinkTime;
    private float eyesBaseScaleY;
    private Vector3 ringBaseScale;

    private void Awake()
    {
        eyesBaseScaleY = eyes.localScale.y;
        ringBaseScale = ring.transform.localScale;
        ScheduleBlink();
    }

    public void Jumped() => Kick(jumpStretch);

    public void Landed() => Kick(landSquash);

    private void Kick(Vector2 target)
    {
        scale = target;
        scaleVelocity = Vector2.zero;
    }

    private void Update()
    {
        // Damped spring back to round. The pivot is the orb's centre, so shift it to keep the bottom planted.
        Vector2 force = (Vector2.one - scale) * springStiffness - scaleVelocity * springDamping;
        scaleVelocity += force * Time.deltaTime;
        scale += scaleVelocity * Time.deltaTime;
        transform.localScale = new Vector3(scale.x, scale.y, 1f);
        transform.localPosition = new Vector3(0f, (scale.y - 1f) * radius, 0f);

        float pulse = 1f + Mathf.Sin(Time.time * ringPulseSpeed) * ringPulseAmount;
        ring.transform.localScale = ringBaseScale * pulse;
        ring.transform.Rotate(0f, 0f, -90f * Time.deltaTime);

        Color color = core.color;
        Color ringColor = color;
        ringColor.a = 0.7f;
        ring.color = ringColor;
        Color trailStart = color;
        trailStart.a = 0.6f;
        Color trailEnd = color;
        trailEnd.a = 0f;
        trail.startColor = trailStart;
        trail.endColor = trailEnd;

        UpdateBlink();
    }

    private void UpdateBlink()
    {
        float sinceBlink = Time.time - nextBlinkTime;
        float open = sinceBlink < 0f || sinceBlink > blinkDuration ? 1f : 0.1f;
        Vector3 eyeScale = eyes.localScale;
        eyeScale.y = eyesBaseScaleY * open;
        eyes.localScale = eyeScale;

        if (sinceBlink > blinkDuration) ScheduleBlink();
    }

    private void ScheduleBlink()
    {
        nextBlinkTime = Time.time + Random.Range(blinkInterval.x, blinkInterval.y);
    }
}
