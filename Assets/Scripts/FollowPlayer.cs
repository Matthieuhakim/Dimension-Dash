using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField]
    private float delay = 1.5f;

    private PlayerController playerController;

    private float speed;

    void Awake()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        speed = playerController.playerSpeed - delay;
    }

    void Update()
    {
        if (GameManager.Instance.isPlaying)
        {
            transform.Translate(speed * Time.deltaTime * Vector2.right);
        }

    }
}
