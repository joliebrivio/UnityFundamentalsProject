using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset = new(0f, 0f, -20f);
    [SerializeField] private float smoothTime = 0.15f;
    private Vector3 velocity;


    // Update is called once per frame
    void Update()
    {
        Vector3 targetPos = target.position + offset;
        targetPos.z = transform.position.z;
        targetPos.y = transform.position.y;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothTime);
    }
}
