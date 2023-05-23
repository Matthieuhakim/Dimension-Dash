using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private float xOffset = 7;
    private float yOffset = 2;

    [SerializeField]
    private Transform playerTransform;

    void Awake()
    {
        SetPosition(playerTransform.position.x + xOffset, playerTransform.position.y + yOffset);
    }


    void LateUpdate()
    {

        float xPosition = playerTransform.position.x + xOffset;
        float yPosition = transform.position.y;

        //TODO: follow y position with delay
        SetPosition(xPosition, yPosition);
    }


    public void SetPosition(float x, float y)
    {
        transform.position = new Vector3(x, y, -10);
    }
}
