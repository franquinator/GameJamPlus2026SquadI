using UnityEngine;

// Se pone en los prefabs de power-up, que son solo eso: flotan en el mapa y
// cuando el jugador los toca le entregan el poder y desaparecen.
// El proyectil que sale disparado es otro prefab distinto (Proyectil*).
[RequireComponent(typeof(Collider2D))]
public class PowerUpPickup : MonoBehaviour
{
    public PowerUpType tipo = PowerUpType.Tijeras;

    void Awake()
    {
        Collider2D colision = GetComponent<Collider2D>();
        if (colision != null && !colision.isTrigger) colision.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerAtaque ataque = other.GetComponent<PlayerAtaque>();
        if (ataque == null) return;

        ataque.RecogerPowerUp(tipo);
        Destroy(gameObject);
    }
}