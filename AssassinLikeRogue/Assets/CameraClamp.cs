using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClamp : MonoBehaviour
{
    [SerializeField] private float minX, maxX, minY, maxY;
    [SerializeField] private Transform targetToFollow;

    [SerializeField] private float dampTime = 0.4f;
    [SerializeField] private Vector3 cameraPos;
    [SerializeField] private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        cameraPos = new Vector3(targetToFollow.position.x, targetToFollow.position.y, -10f);
        cameraPos = Vector3.SmoothDamp(gameObject.transform.position, cameraPos, ref velocity, dampTime);
        transform.position = new Vector3(
            Mathf.Clamp(cameraPos.x, minX, maxX),
            Mathf.Clamp(cameraPos.y, minY, maxY),
            cameraPos.z);
    }

    //void Update()
    //{
    //    transform.position = new Vector3(
    //        Mathf.Clamp(targetToFollow.position.x, minX, maxX), 
    //        Mathf.Clamp(targetToFollow.position.y, minY, maxY),
    //        transform.position.z);
    //}
}
