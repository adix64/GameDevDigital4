using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    public Transform jucator;
    //https://upload.wikimedia.org/wikipedia/commons/thumb/0/04/Flight_dynamics_with_text_ortho.svg/640px-Flight_dynamics_with_text_ortho.svg.png?1605627684091
    float yaw = 0f; //unghiul facut de axele camerei cu axa verticala a lumii
    float pitch = 0f; //unghiul facut de axele camerei cu axa orizontala a lumii
    public float distanceToTarget = 4f;
    public float minPitch = -45f, maxPitch = 45f; // limitele rotatiei sus-jos, a.i. sa nu se dea camera peste cap
  

    // Update is called once per frame
    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X"); //deplasament orizontal mouse
        pitch -= Input.GetAxis("Mouse Y"); //deplasament vertical mouse

        // limitare rotatie sus-jos, a.i. sa nu se dea camera peste cap
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        //efectuam rotatia:
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        //pentru camera third person:
        //plasam camera in pozita personajului si ne dem in spate distanceToTarget unitati:
        transform.position = jucator.position - transform.forward * distanceToTarget;
    }
}
