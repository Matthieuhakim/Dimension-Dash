using UnityEngine;

// Fits this RectTransform to the device safe area (notches, rounded corners, home indicator).
// Put UI content under it; leave full-screen backgrounds outside it.
[RequireComponent(typeof(RectTransform))]
public class SafeArea : MonoBehaviour
{
    private RectTransform rectTransform;
    private Rect appliedArea;
    private Vector2Int appliedScreen;

    private void Awake()
    {
        rectTransform = (RectTransform)transform;
        Apply();
    }

    private void Update()
    {
        if (Screen.safeArea != appliedArea || Screen.width != appliedScreen.x || Screen.height != appliedScreen.y)
        {
            Apply();
        }
    }

    private void Apply()
    {
        appliedArea = Screen.safeArea;
        appliedScreen = new Vector2Int(Screen.width, Screen.height);
        if (Screen.width <= 0 || Screen.height <= 0) return;

        rectTransform.anchorMin = new Vector2(appliedArea.xMin / Screen.width, appliedArea.yMin / Screen.height);
        rectTransform.anchorMax = new Vector2(appliedArea.xMax / Screen.width, appliedArea.yMax / Screen.height);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
