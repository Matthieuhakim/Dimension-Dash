using UnityEngine;

// Background layer that scrolls slower than the world. Use a Tiled SpriteRenderer at least one tile wider than
// the widest view: the layer stays centred on the camera and its pattern is shifted by whole-tile wrapping.
// Runs after CameraController so it uses this frame's camera position.
[DefaultExecutionOrder(100)]
public class ParallaxLayer : MonoBehaviour
{
    [Tooltip("0 = fixed to the camera (infinitely far), 1 = moves with the world.")]
    [SerializeField, Range(0f, 1f)]
    private float scroll = 0.2f;
    [Tooltip("Width of one repeat of the sprite pattern, in world units. 0 = no horizontal wrapping (static backdrop).")]
    [SerializeField]
    private float tileWidth;

    private Transform cam;
    private Vector3 offset;
    private float startCamY;

    private void Start()
    {
        cam = Camera.main.transform;
        offset = transform.position - cam.position;
        startCamY = cam.position.y;
    }

    private void LateUpdate()
    {
        float x = cam.position.x + offset.x;
        if (tileWidth > 0f)
        {
            x -= Mathf.Repeat(cam.position.x * scroll, tileWidth);
        }

        float y = cam.position.y + offset.y - (cam.position.y - startCamY) * scroll;
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
