using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNight : MonoBehaviour
{
    public Transform sun;
    public float cycleInMinutes = 1;

    void Start()
    {
        sun = transform;
        sun.rotation = Quaternion.identity;
    }

    void Update()
    {
        RotateSun();
    }

    void RotateSun()
    {
        // Rotate 360 degrees every cycleInMinutes minutes.
        sun.Rotate(Vector3.right * Time.deltaTime * 6 / cycleInMinutes);
    }
}
