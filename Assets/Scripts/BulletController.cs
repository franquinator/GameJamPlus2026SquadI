using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float velocidad;
    public float daño;

    [Header("Dueño")]
    // ShootController lo copia al instanciar la bala, así sabe a quién no debe dañar
    public int playerNumber = 1;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector2.right * velocidad * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController objetivo = other.GetComponent<PlayerController>();

            // Solo daña al otro jugador. Si le pega a quien la disparó no le hace daño y sigue de largo
            if (objetivo != null && objetivo.playerNumber != playerNumber)
            {
                HealthManager salud = other.GetComponent<HealthManager>();
                if (salud != null)
                {
                    salud.RecibirDaño();
                    Destroy(gameObject);
                }
            }

            return;
        }

        Destroy(gameObject);
    }
}
