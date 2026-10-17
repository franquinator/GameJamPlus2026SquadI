using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FlagController : MonoBehaviour
{
    SpriteRenderer colorFlag;
    // Start is called before the first frame update
    void Start()
    {
        colorFlag = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Accedes al tag a través de other.tag o other.gameObject.tag
        
        Debug.Log("¡Colisión con " + other.gameObject.tag);
        Color colorTagNew = other.GetComponent<Color>();
        GetComponent<SpriteRenderer>().color = colorTagNew;
    }
}

