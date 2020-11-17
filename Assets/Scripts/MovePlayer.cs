using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float moveSpeed = 2f; // viteza de deplasare a personajului
    public float rotSpeed = 4f; // viteza de orientare a personajului
    public Transform cameraTransform;
    Rigidbody rigidbody;
    Vector3 moveDir; // directia deplasarii personajului, in World Space
    // Apelat o singura data, la inceputul jocului sau cand obiectul e activat/spawnat
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    // Update e apelat de N ori pe secunda, N fluctuant, preferabil N > 60FPS
    void Update()
    {
        GetMovementDirection();
        ApplyRootMotion();
        ApplyRootRotation();
        //ApplyRootRotationSimple(dir);
    }

    private void ApplyRootRotationSimple(Vector3 dir)
    {
        Quaternion lookAtDir = Quaternion.LookRotation(dir); // rotatia dorita, ce aliniaza personajul cu fata catre moveDir
        // rotatia animata prin interpolare catre directia dorita
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtDir, Time.deltaTime * rotSpeed);
    }

    private void ApplyRootRotation()
    {
        Vector3 F = transform.forward; // directia in care privestre personajul, vector mobil
        Vector3 D = moveDir; // directia de aliniat personajul, vector fix

        Vector3 FminusD = F - D; // daca vectorii sunt confundati, atunci diferenta este aproape 0
        Vector3 FplusD = F + D; // daca vectorii sunt opusi, atunci suma este aproape 0

        if (FminusD.magnitude > 0.0001f && FplusD.magnitude > 0.0001f) // comparatie cu epsilon, un nr. mic
        { // efectuam rotatia doar in cazul in care suma/diferenta sunt diferite de 0
            float u = Mathf.Acos(Vector3.Dot(F, D)); // unghiul dintre cele 2 directii
            u *= Mathf.Rad2Deg; // transformat din radiani in grade
            Vector3 axis = Vector3.Cross(F, D); // axa de rotatie, produsul vectorial, perpedicular pe cele 2 directii
            //efectuam rotatia:
            transform.rotation = Quaternion.AngleAxis(u * Time.deltaTime * rotSpeed, axis) * transform.rotation;
        }
        if (FplusD.magnitude < 0.0001f) // pentru cand directiile sunt opuse, rotim cu un pas mic, a.i. sa nu mai fie opuse
            transform.rotation = Quaternion.AngleAxis(Time.deltaTime * rotSpeed, Vector3.up) * transform.rotation;
    }

    private void ApplyRootMotion()
    {
        //deplasamentul trebuie sa fie proportional cu timpul scurs intre 2 cadre...
        //...ca sa putem mentine viteza independent de framerate
        //doar pentru non - rigidbody:
            //Vector3 offset = dir * Time.deltaTime * moveSpeed; //deplasamentul intre cadre...
            // transform.position += offset; //se aduna la pozitia personajului
        float velY = rigidbody.velocity.y;
        rigidbody.velocity = moveDir * moveSpeed;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x,
                                         velY, // se pastreaza doar componenta verticala ca sa cada fizic
                                         rigidbody.velocity.z);
    }
    private void GetMovementDirection()
    {
        float h = Input.GetAxis("Horizontal"); // -1 pentru tasta A, 1 pentru tasta D, 0 altfel
        float v = Input.GetAxis("Vertical"); // -1 pentru tasta S, 1 pentru tasta W, 0 altfel
        //directia de miscare relativa la orientarea camerei
        moveDir = h * cameraTransform.right +
                  v * cameraTransform.forward;
        moveDir.y = 0f; // miscarea se face doar in planul orizontal, xOz
        moveDir = moveDir.normalized; // lungime 1 pentru orice Vector3 ce reprezinta directie
       
    }
}
