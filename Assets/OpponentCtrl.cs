using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentCtrl : MonoBehaviour
{
    UnityEngine.AI.NavMeshAgent agent;
    Animator animator;
    public Transform player;
    public float attackDistanceThresh = 1.5f;
    Vector3 destinationOffset;
    float phase;
    AnimatorStateInfo stateInfo;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        StartCoroutine(SeedMoveDirection(1f));
        phase = UnityEngine.Random.Range(0f, Mathf.PI);
    }
    //schimba periodic(o data la t secunde) comportamentul oponentului
    IEnumerator SeedMoveDirection(float t)
    {
        yield return new WaitForSeconds(t);
        //schimba directia de miscare
        int offsetType = UnityEngine.Random.Range(0, 10);
        switch (offsetType)
        {
            case 0: case 1: case 2:// ocol player prin stanga
                destinationOffset = -player.right * 3f; 
                break;
            case 3: case 4: case 5:// ocol player prin dreapta
                destinationOffset = player.right * 3f;
                break;
            case 6: case 7: case 8:// retragere
                destinationOffset = player.forward * 3f;
                break;
            case 9:// fara offset, du-te direct la target
                destinationOffset = Vector3.zero;
                break;
        }

        float newT = UnityEngine.Random.Range(0.5f, 2.5f);
        yield return StartCoroutine(SeedMoveDirection(newT));//recursiva
    }

    // Update is called once per frame
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Die"))
            agent.SetDestination(transform.position);// e mort, sta pe loc
        else
            agent.SetDestination(player.position + destinationOffset); //urmareste/ocoleste playerul

        SetAnimatorParameters();

        attackDistanceThresh = (Mathf.Sin(Time.time + phase) + 1f); //oscileaza in 0..2
        if (Vector3.Distance(transform.position, player.position) < attackDistanceThresh)
            animator.SetTrigger("Attack");//ataca doar daca e suficient de aproape
    }

    private void SetAnimatorParameters()
    {
        Vector3 characterSpaceDir = transform.InverseTransformDirection(agent.velocity);
        animator.SetFloat("Forward", characterSpaceDir.z, 0.1f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceDir.x, 0.1f, Time.deltaTime);
    }

    private void LateUpdate()
    {
        if (stateInfo.IsName("Die"))
            return; //daca e mort nu se mai roteste

        Vector3 D = (player.position - transform.position);
        D.y = 0f;
        D = D.normalized;
        Quaternion lookAtDir = Quaternion.LookRotation(D); // rotatia dorita, ce aliniaza personajul cu fata catre moveDir
        // rotatia animata prin interpolare catre directia dorita
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtDir, Time.deltaTime * 10f);
    }
}
