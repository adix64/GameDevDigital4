using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float moveSpeed = 2f; // viteza de deplasare a personajului
    public float rotSpeed = 4f; // viteza de orientare a personajului
    public float jumpPower = 4f;
    public float groundedThreshold = 0.1f;
    public float minY = -30f;

    public Transform cameraTransform;
    Rigidbody rigidbody;
    Animator animator;
    CapsuleCollider capsule;
    Vector3 moveDir; // directia deplasarii personajului, in World Space
    Transform enemy;
    Vector3 initialPos;

    bool movingWith = false;
    Collider moveWithCollider;
    Rigidbody moveWithRigidbody;

    public Transform enemyContainer;
    List<Transform> enemies;
    AnimatorStateInfo stateInfo;
    // Apelat o singura data, la inceputul jocului sau cand obiectul e activat/spawnat
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsule  = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        initialPos = transform.position;
        enemies = new List<Transform>();
        for (int i = 0; i < enemyContainer.childCount; i++)
            enemies.Add(enemyContainer.GetChild(i));
    }
    // Update e apelat de N ori pe secunda, N fluctuant, preferabil N > 60FPS
    void Update()
    {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        GetMovementDirection();
        UpdateAnimatorParameters();
        ApplyRootMotion();
        ApplyRootRotationSimple();
        HandleMidair();
        HandleAttack();
    }

    private void HandleAttack()
    {
        HandleGuardAnimation();

        if (Input.GetButtonDown("Fire1"))
            animator.SetTrigger("Attack");
    }

    private void HandleGuardAnimation()
    {
        if (stateInfo.IsName("Grounded") && enemy != null)
        {//poate ridica garda doar in state-ul grounded daca exista inamic
            float dist = Vector3.Distance(enemy.position, transform.position);
            //distanta dintre oponenti e invers proportionala cu garda
            float guardWeight = 1f - (Mathf.Clamp(dist, 2f, 4f) - 2f) / 2;
            animator.SetLayerWeight(1, guardWeight); //setam influenta gardei
        }
        else
        {//altfel influenta gardei e zero
            animator.SetLayerWeight(1, 0f);
        }
    }

    private void HandleMidair()
    {
        bool midair = true;
        for (float xOffset = -1f; xOffset <= 1f; xOffset += 1f)
        {
            for (float zOffset = -1f; zOffset <= 1f; zOffset += 1f)
            {
                Vector3 offset = new Vector3(xOffset, 0, zOffset).normalized * capsule.radius;
                Ray ray = new Ray(transform.position + Vector3.up * groundedThreshold + offset, Vector3.down);
        
                if (Physics.Raycast(ray, 2f * groundedThreshold))
                {//pe pamant
                    midair = false;
                    break;
                }
            }
        }

        if(midair)
        {//in aer
            animator.SetBool("Midair", true);
        }
        else
        {//pe sol
            animator.SetBool("Midair", false);

            if (Input.GetButtonDown("Jump"))
            {//poti sari doar daca esti pe sol
                Vector3 jumpForce = (Vector3.up + moveDir) * jumpPower;
                rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
            }
        }

        if (transform.position.y < minY)
        {//teleporteaza player la pozitia initiala daca a cazut suficient de jos
            transform.position = initialPos;
        }
    }
    private void UpdateAnimatorParameters()
    {
        Vector3 characterSpaceDir = transform.InverseTransformDirection(moveDir);
        animator.SetFloat("Forward", characterSpaceDir.z, 0.1f, Time.deltaTime);
        animator.SetFloat("Right", characterSpaceDir.x, 0.1f, Time.deltaTime);
    }
    private void ApplyRootRotationSimple()
    {
        if (animator.GetBool("Midair"))// || stateInfo.IsTag("attack"))
            return;
        Vector3 D = GetClosestEnemyDirection();
        Quaternion lookAtDir = Quaternion.LookRotation(D); // rotatia dorita, ce aliniaza personajul cu fata catre moveDir
        // rotatia animata prin interpolare catre directia dorita
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtDir, Time.deltaTime * rotSpeed);
    }
   
    private Vector3 GetClosestEnemyDirection()
    {
        Vector3 D = moveDir; //initial directia de privire coincide cu cea de deplasare
        float minDist = float.MaxValue;
        int closestEnemyIndex = -1;
        for (int i = 0; i < enemies.Count; i++)
        {//compara distantele dintre player si fiecare inamic ca sa obtii cel mai apropiat inamic
            float dist = Vector3.Distance(transform.position, enemies[i].position);
            if (dist < 4f && dist < minDist)
            {
                minDist = dist;
                closestEnemyIndex = i;
            }
        }
        if (closestEnemyIndex != -1)
        {//daca exista cel mai apropiat inamic la mai putin de 4 metri
            enemy = enemies[closestEnemyIndex];
            D = enemy.position - transform.position; //directia de privire pointeaza la inamic
            D.y = 0f; //directie in plan orizontal
            D = D.normalized;
        }
        return D;
    }
    private void ApplyRootMotion()
    {
        //deplasamentul trebuie sa fie proportional cu timpul scurs intre 2 cadre...
        //...ca sa putem mentine viteza independent de framerate
        //doar pentru non - rigidbody:
        //Vector3 offset = dir * Time.deltaTime * moveSpeed; //deplasamentul intre cadre...
        // transform.position += offset; //se aduna la pozitia personajului

        if (animator.GetBool("Midair"))
        {//nu imprima root motion, in aer, lasa rigidbody physics sa se ocupe de miscare
            animator.applyRootMotion = false;
            return;
        }else
            animator.applyRootMotion = true;

        float velY = rigidbody.velocity.y;
        Vector3 fidelityDir = animator.deltaPosition.magnitude * moveDir.normalized;
        rigidbody.velocity = fidelityDir / Time.deltaTime;//moveDir * moveSpeed;
        if (movingWith)
        {//daca e platforma cu care se misca impreuna
            if (rigidbody.velocity.magnitude > 0.025f)
            {//daca incerci sa te misti din taste/joystick, atunci fa frecare mica cu platforma
                moveWithCollider.material.staticFriction = 20f;
            }
            else
            {//altfel frecare mare ca sa se miste impreuna cu ea
                moveWithCollider.material.staticFriction = 200f;
                rigidbody.velocity += moveWithRigidbody.velocity;
            }
        }
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
    private void OnCollisionEnter(Collision collision)
    {//functie declansata automat in cadrul in care incepe coliziunea
        if (collision.rigidbody.gameObject.layer == LayerMask.NameToLayer("MoveWith"))
        {//coliziune cu platforma care te poarta pe ea (se afla pe layer "MoveWith")
            movingWith = true; // incepe deplasarea impreuna cu platforma
            moveWithCollider = collision.collider;
            moveWithRigidbody = collision.rigidbody;
        }
    }
    private void OnCollisionExit(Collision collision)
    {//functie declansata automat in cadrul in care se termina coliziunea
        if (collision.rigidbody.gameObject.layer == LayerMask.NameToLayer("MoveWith"))
            movingWith = false;// se incheie deplasarea impreuna cu platforma
    }
}
