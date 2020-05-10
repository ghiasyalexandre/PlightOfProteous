using System;
using UnityEngine;

public class LerpHelper : MonoBehaviour
{
    [SerializeField] private bool shouldLerp = false;
    [SerializeField] private float currentLerpTime;
    [SerializeField] private float lerpTime;
    [SerializeField] private float distance;

    public Vector3 startPosition;
    public Vector3 endPosition;
    //public Vector3 direction;

    //public float Distance { set { distance = value; } }
    //public float LerpTime { set { lerpTime = value; } }
    //public bool ShouldLerp { set { shouldLerp = value; } }
    //public Vector3 Direction { set { direction = value; } }

    //private void StartLerping()
    //{
    //    currentLerpTime = Time.time;

    //    shouldLerp = true;
    //}

    //private void Update()
    //{
    //    if (shouldLerp)
    //    {
    //        startPosition = transform.position;
    //        endPosition = transform.position + (direction * distance);
    //        transform.position = Lerp(startPosition, endPosition, currentLerpTime, lerpTime);
    //    }
    //}

    public Vector3 Lerp(Vector3 start, Vector3 end, float timeStartedLerping, float lerpTime = 1)
    {
        float timeSinceStarted = Time.time - timeStartedLerping;

        float percentageComplete = timeSinceStarted / lerpTime;

        var result = Vector3.Lerp(start, end, percentageComplete);

        return result;
    }
}
