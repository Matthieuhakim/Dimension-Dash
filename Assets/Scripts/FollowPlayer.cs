using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField]
    private float delay = 1.5f;

    private PlayerController playerController;

    private float speed;

    // Start is called before the first frame update
    void Awake()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        speed = playerController.playerSpeed - delay;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.isPlaying)
        {
            transform.Translate(speed * Time.deltaTime * Vector2.right);
        }

    }
}
