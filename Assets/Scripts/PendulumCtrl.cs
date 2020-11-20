using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumCtrl : MonoBehaviour
{
    public float freq = 2f;
    public float amplitude = 45f;

    void Update()
    {
        float rotX = Mathf.Sin(Time.time * freq) * amplitude;
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);
    }
}
