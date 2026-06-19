using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public Transform target; // игрок

    public float verticalOffset = 2f; // насколько камера выше игрока
    public float smoothSpeed = 5f; // плавность

    public float minY = 0f; // нижняя граница
    public float maxY = Mathf.Infinity; // верхняя граница

    public float deadZoneX = 2f; //чувствительность по оси x
    public float targetPositionX; //предыдущая позиция игрока

    void Start()
    {
        if (target != null)
        {
            targetPositionX = target.position.x;
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }

        float targetY = target.position.y + verticalOffset;

        float distanceX = target.position.x - targetPositionX;

        // Если игрок ушел слишком далеко вправо, двигаем нашу цель вправо
        if (distanceX > deadZoneX)
        {
            targetPositionX = target.position.x - deadZoneX;
        }
        // Если игрок ушел слишком далеко влево, двигаем нашу цель влево
        else if (distanceX < -deadZoneX)
        {
            targetPositionX = target.position.x + deadZoneX;
        }

        targetY = Mathf.Clamp(targetY, minY, maxY);

        float newY = Mathf.Lerp(transform.position.y, targetY, smoothSpeed * Time.deltaTime);
        float newX = Mathf.Lerp(transform.position.x, targetPositionX, smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(newX, newY, transform.position.z);
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
            targetPositionX = target.position.x;
        }
    }
}
