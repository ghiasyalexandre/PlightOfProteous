using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClamp : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;
    private float x, X, y, Y;
    private float oldMinX, oldMaxX, oldMinY, oldMaxY;
    private float newMinX, newMaxX, newMinY, newMaxY;
    private bool isLerping;
    private float startTime = 0f;
    private float lerpSpeed = 5f;
    private float lerpTime = 1f; 
    [SerializeField] private float dampTime = 0.2f;
    private Vector3 cameraPos;
    private Vector3 velocity = Vector3.zero;

    private void Start()
    {
        SetClamp(-0.9f, 0.9f, -0.78f, 0.78f);
        //x = y = -1;
        //X = Y = 1;
    }

    public void SetClamp(float _minX, float _maxX, float _minY, float _maxY, bool lerp = false)
    {
        //oldMinX = newMinX;
        //oldMaxX = newMaxX;
        //oldMinY = newMinY;
        //oldMaxY = newMaxY;

        newMinX = _minX;
        newMaxX = _maxX;
        newMinY = _minY;
        newMaxY = _maxY;
        isLerping = true;
    }
    
    private void LateUpdate()
    {
        cameraPos = new Vector3(targetToFollow.position.x, targetToFollow.position.y, -10f);
        cameraPos = Vector3.SmoothDamp(gameObject.transform.position, cameraPos, ref velocity, dampTime);

        if (isLerping)
        {
            startTime += Time.deltaTime * lerpSpeed;
            if (startTime <= lerpTime)
            {
                oldMinX = Mathf.Lerp(oldMinX, newMinX, startTime);
                oldMaxX = Mathf.Lerp(oldMaxX, newMaxX, startTime);
                oldMinY = Mathf.Lerp(oldMinY, newMinY, startTime);
                oldMaxY = Mathf.Lerp(oldMaxY, newMaxY, startTime);

                //x = Mathf.Lerp(oldMinX, newMinX, startTime);
                //y = Mathf.Lerp(oldMinY, newMinY, startTime);
                //X = Mathf.Lerp(oldMaxX, newMaxX, startTime);
                //Y = Mathf.Lerp(oldMaxY, newMaxY, startTime);
                startTime = 0f;
            }
            else
                isLerping = false;
        }

        transform.position = new Vector3(
            Mathf.Clamp(cameraPos.x, oldMinX, oldMaxX),
            Mathf.Clamp(cameraPos.y, oldMinY, oldMaxY),
            cameraPos.z);

        //transform.position = new Vector3(
        //    Mathf.Clamp(cameraPos.x, x, X),
        //    Mathf.Clamp(cameraPos.y, y, Y),
        //    cameraPos.z);
    }
}
