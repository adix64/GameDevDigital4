using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevitateCtrl : MonoBehaviour
{
    float initialY; //pozitia initiala pe axa verticala
    float initialX; //pozitia initiala pe axa orizontala
  
    public float verticalFreq = 1f; // cat de repede oscileaza levitatorul sus-jos
    public float verticalAmplitude = 0.25f; //cat de larg oscileaza

    public float horizontalFreq = 1f; // cat de repede oscileaza levitatorul sus-jos
    public float horizontalAmplitude = 0.25f; //cat de larg oscileaza
    Rigidbody rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        initialX = transform.position.x;
        initialY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        float posX = Mathf.Sin(Time.time * horizontalFreq) * horizontalAmplitude; // oscilatia pe orizontala
        float posY = Mathf.Sin(Time.time * verticalFreq) * verticalAmplitude; // oscilatia pe verticala

        Vector3 newPosition = new Vector3(initialX + posX,
                                    initialY + posY, // doar coordonata verticala se schimba
                                    transform.position.z);
        rigidbody.velocity = (newPosition - transform.position) / Time.deltaTime; // deplasament la noua pozitie impartit la timp
    }
}
