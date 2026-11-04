using UnityEngine;

// Marks a trigger (a pad runtime layer) that launches the player upwards on contact.
// Combine with DimensionObject to make it exist in one dimension only.
public class LaunchPad : MonoBehaviour
{
    [Tooltip("Upward velocity given to the player. The player's jump is 16.")]
    [SerializeField]
    private float launchVelocity = 23f;

    public float LaunchVelocity => launchVelocity;
}
