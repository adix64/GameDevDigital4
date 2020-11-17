using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowsCtrl : MonoBehaviour
{
    GameObject L, R, F, B;
    // Start is called before the first frame update
    void Start()
    {
        F = transform.GetChild(0).gameObject;
        B = transform.GetChild(1).gameObject;
        L = transform.GetChild(2).gameObject;
        R = transform.GetChild(3).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        F.SetActive(Input.GetAxis("Vertical") > 0f);
        B.SetActive(Input.GetAxis("Vertical") < 0f);
        L.SetActive(Input.GetAxis("Horizontal") < 0f);
        R.SetActive(Input.GetAxis("Horizontal") > 0f);
    }
}
