using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCtrl : MonoBehaviour
{
    MeshRenderer meshRenderer;
    public Transform cameraTransform;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 camToBox = transform.position - cameraTransform.position; // directia de la camera la cutie
        //cosinusul unghiului dintre directia camera-cutie si directia de privire a camerei:
        float dotProduct = Vector3.Dot(cameraTransform.forward, camToBox.normalized); //in [-1, 1]
        dotProduct = (dotProduct + 1f) * .5f; //in [0, 1]
        float f = Mathf.Pow(dotProduct, 16f);// factor de interpolare intre negru(away) si galben(pe directia de privire)
        // cu cat e mai mare puterea (e.g. >16f), va fi mai concentrata culoarea galben pe obiectul din centrul de privire
        meshRenderer.material.color = Color.Lerp(Color.black, Color.yellow, f);
    }
}
