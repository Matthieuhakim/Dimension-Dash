using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UiButtons
{
    // Click sound, then the action. Selection is cleared so jump inputs (gamepad South is also Submit)
    // can't press the button again.
    public static void OnClick(this Button button, Action action)
    {
        button.onClick.AddListener(() =>
        {
            SoundManager.Play(SoundManager.Sfx.Click);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            action();
        });
    }
}
