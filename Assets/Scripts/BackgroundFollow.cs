using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private Vector2 offset;

    // Update is called once per frame
    private void Update()
    {
        Vector3 transformPosition = mainCamera.transform.position;
        transformPosition.x += offset.x;
        transformPosition.y += offset.y;
        transformPosition.z = transform.position.z;

        // We do not want to change the z axis therefore convert into Vector2.
        transform.position = transformPosition;
    }
}
