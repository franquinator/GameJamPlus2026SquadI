using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float velocidad;
    public float daño;
    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            //other.recibirDaño(daño);  HAY QUE DEFINIR LA FUNCION RECIBIR DAÑO EN EL PLAYERCONTROLLER
            Destroy(gameObject);
        }
        else { Destroy(gameObject); }
    }
}
