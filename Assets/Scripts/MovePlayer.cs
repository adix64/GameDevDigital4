using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float moveSpeed = 2f; // viteza de deplasare a personajului
  
    // Apelat o singura data, la inceputul jocului sau cand obiectul e activat/spawnat
    void Start()
    {
    }
    // Update e apelat de N ori pe secunda, N fluctuant, preferabil N > 60FPS
    void Update()
    {
        float h = Input.GetAxis("Horizontal"); // -1 pentru tasta A, 1 pentru tasta D, 0 altfel
        float v = Input.GetAxis("Vertical"); // -1 pentru tasta S, 1 pentru tasta W, 0 altfel

        Vector3 dir = new Vector3(h, 0, v);
        dir = dir.normalized; // lungime 1 pentru orice Vector3 ce reprezinta directie

        //deplasamentul trebuie sa fie proportional cu timpul scurs intre 2 cadre...
        //...ca sa putem mentine viteza independent de framerate
        Vector3 offset = dir * Time.deltaTime * moveSpeed; //deplasamentul intre cadre...
        transform.position += offset; //se aduna la pozitia personajului
    }
}
