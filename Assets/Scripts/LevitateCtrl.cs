using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitateCtrl : MonoBehaviour
{
    float initialY; //pozitia initiala pe axa verticala
    public float freq = 1f; // cat de repede oscileaza levitatorul sus-jos
    public float amplitude = 0.25f; //cat de larg oscileaza

    // Start is called before the first frame update
    void Start()
    {
        initialY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float posY = initialY + Mathf.Sin(Time.time * freq) * amplitude; // oscilatia pe verticala

        transform.position = new Vector3(transform.position.x,
                                        posY, // doar coordonata verticala se schimba
                                        transform.position.z);
    }
}
