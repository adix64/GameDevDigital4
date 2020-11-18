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

    Vector3 initialPos;

    bool movingWith = false;
    Collider moveWithCollider;
    Rigidbody moveWithRigidbody;
    // Apelat o singura data, la inceputul jocului sau cand obiectul e activat/spawnat
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        capsule  = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        initialPos = transform.position;
    }
    // Update e apelat de N ori pe secunda, N fluctuant, preferabil N > 60FPS
    void Update()
    {
        GetMovementDirection();
        UpdateAnimatorParameters();
        ApplyRootMotion();
        ApplyRootRotation();
        //ApplyRootRotationSimple(dir);
        HandleMidair();
        HandleAttack();
    }

    private void HandleAttack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            animator.SetTrigger("Attack");
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
    private void ApplyRootRotationSimple(Vector3 dir)
    {
        Quaternion lookAtDir = Quaternion.LookRotation(dir); // rotatia dorita, ce aliniaza personajul cu fata catre moveDir
        // rotatia animata prin interpolare catre directia dorita
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtDir, Time.deltaTime * rotSpeed);
    }

    private void ApplyRootRotation()
    {
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (animator.GetBool("Midair") || stateInfo.IsTag("attack") )
            return;
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

        if (animator.GetBool("Midair"))
        {
            animator.applyRootMotion = false;
            return;
        }else
            animator.applyRootMotion = true;

        float velY = rigidbody.velocity.y;
        rigidbody.velocity = animator.deltaPosition / Time.deltaTime;//moveDir * moveSpeed;
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
