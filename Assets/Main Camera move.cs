using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameramove : MonoBehaviour
{
    public Transform target; // 따라갈 대상(플레이어)
    public float smoothSpeed = 0.125f;
    public Vector3 offset; // 카메라와 플레이어 사이 거리

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPostion = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPostion;
    }
}
