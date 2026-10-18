using UnityEngine;

public class ShootController : MonoBehaviour
{
    public int playerNumber = 1; // Número del jugador (1 o 2)
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float fireRate = 0.5f;

    [Header("Apuntado hacia arriba")]
    // Mientras se mantiene la tecla de salto el disparo sale hacia arriba
    public float alturaDisparoArriba = 1f;
    public float anguloDisparoArriba = 90f;

    private float nextFireTime = 0f;

    // Posición y rotación del firePoint en el disparo normal, para poder restaurarlas
    private Vector3 posicionNormal;
    private Quaternion rotacionNormal;

    void Awake()
    {
        if (firePoint != null)
        {
            posicionNormal = firePoint.localPosition;
            rotacionNormal = firePoint.localRotation;
        }
    }

    void Update()
    {
        // La tecla de salto (up / w) también sirve para apuntar hacia arriba
        Apuntar(Input.GetButton("Jump" + playerNumber));

        if (Input.GetButtonDown("Fire" + playerNumber) && Time.time >= nextFireTime)
        {
            AudioManager.Instance.PlayDisparo();
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void Apuntar(bool arriba)
    {
        if (firePoint == null) return;

        if (arriba)
        {
            // Se sube el punto de salida para que la bala no nazca al costado del personaje
            firePoint.localPosition = new Vector3(0f, alturaDisparoArriba, 0f);
            firePoint.localRotation = Quaternion.Euler(0f, 0f, anguloDisparoArriba);
        }
        else
        {
            firePoint.localPosition = posicionNormal;
            firePoint.localRotation = rotacionNormal;
        }
    }


    void Shoot()
    {
        BulletController bala = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation).GetComponent<BulletController>();
        // La bala guarda de qué jugador salió para poder no dañarlo
        bala.playerNumber = playerNumber;
    }
}