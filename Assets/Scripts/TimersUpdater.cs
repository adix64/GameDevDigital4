using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimersUpdater : MonoBehaviour
{
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //numara cat timp a trecut de cand a fost lovit ultima data
        animator.SetFloat("timeSinceTakenHit",
                    animator.GetFloat("timeSinceTakenHit") + Time.deltaTime);   
    }
}
