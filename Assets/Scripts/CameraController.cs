using UnityEngine;
using UnityEngine.Serialization;

// Runner camera. Fits the view to any aspect ratio (a minimum visible width, so narrow screens zoom out
// instead of losing lookahead), keeps the target at a fixed fraction of the screen width, and follows
// vertically with a dead zone.
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [FormerlySerializedAs("playerTransform")]
    [SerializeField]
    private Transform target;

    [Header("Framing")]
    [Tooltip("World units visible vertically on screens at least as wide as the minimum width.")]
    [SerializeField]
    private float designViewHeight = 10f;
    [Tooltip("World units always visible horizontally, whatever the aspect ratio.")]
    [SerializeField]
    private float minViewWidth = 17.8f;
    [Tooltip("Horizontal screen position of the target, 0 = left edge, 1 = right edge.")]
    [SerializeField, Range(0f, 1f)]
    private float targetScreenX = 0.3f;
    [Tooltip("Height of the view centre above the target's focus point.")]
    [SerializeField]
    private float verticalOffset = 2f;

    [Header("Vertical follow")]
    [Tooltip("How far the target can climb above the focus point before the camera rises. Larger = rises less.")]
    [SerializeField]
    private float deadZoneUp = 3f;
    [Tooltip("How far the target can drop below the focus point before the camera follows down. Smaller = follows tighter.")]
    [SerializeField]
    private float deadZoneDown = 0.5f;
    [Tooltip("Smoothing when the camera moves up. Larger = slower, gentler rise.")]
    [SerializeField]
    private float riseSmoothTime = 0.35f;
    [Tooltip("Smoothing when the camera moves down. Smaller = snaps back faster.")]
    [SerializeField]
    private float fallSmoothTime = 0.08f;
    [Tooltip("Never go lower than the starting height.")]
    [SerializeField]
    private bool clampToStartHeight = true;

    private Camera cam;
    private int screenWidth;
    private int screenHeight;
    private float focusY;
    private float startY;
    private float yVelocity;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        FitToScreen();

        focusY = target.position.y;
        startY = focusY + verticalOffset;
        transform.position = new Vector3(TargetCameraX(), startY, transform.position.z);
    }

    private void LateUpdate()
    {
        if (Screen.width != screenWidth || Screen.height != screenHeight)
        {
            FitToScreen();
        }

        float targetY = target.position.y;
        if (targetY > focusY + deadZoneUp) focusY = targetY - deadZoneUp;
        else if (targetY < focusY - deadZoneDown) focusY = targetY + deadZoneDown;

        float desiredY = focusY + verticalOffset;
        if (clampToStartHeight) desiredY = Mathf.Max(desiredY, startY);

        float smoothTime = desiredY < transform.position.y ? fallSmoothTime : riseSmoothTime;
        float y = Mathf.SmoothDamp(transform.position.y, desiredY, ref yVelocity, smoothTime);
        transform.position = new Vector3(TargetCameraX(), y, transform.position.z);
    }

    private void FitToScreen()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        cam.orthographicSize = Mathf.Max(designViewHeight * 0.5f, minViewWidth * 0.5f / cam.aspect);
    }

    private float TargetCameraX()
    {
        float viewWidth = 2f * cam.orthographicSize * cam.aspect;
        return target.position.x + (0.5f - targetScreenX) * viewWidth;
    }
}
