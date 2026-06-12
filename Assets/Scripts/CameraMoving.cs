using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public Transform target; // игрок

    public float verticalOffset = 2f; // насколько камера выше игрока
    public float smoothSpeed = 5f; // плавность

    public float minY = 0f; // нижняя граница
    public float maxY = Mathf.Infinity; // верхняя граница

    private float fixedX; // фиксированная позиция по X

    void Start()
    {
        fixedX = transform.position.x;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }

        float targetY = target.position.y + verticalOffset;

        targetY = Mathf.Clamp(targetY, minY, maxY);

        float newY = Mathf.Lerp(transform.position.y, targetY, smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(fixedX, newY, transform.position.z);
    }

    public void SetVerticalBounds(float newMinY, float newMaxY)
    {
        minY = newMinY;
        maxY = newMaxY;
    }

    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            target = player.transform;
        }
    }
}
