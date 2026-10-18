using UnityEngine;

public enum PowerUpType { None, Bala, Cinta, Tijeras }

public class PlayerPowerUpShoot : MonoBehaviour
{
    public int playerNumber = 1;

    [Header("Prefabs de los 3 disparos")]
    public GameObject prefabBala;
    public GameObject prefabCinta;
    public GameObject prefabTijeras;

    public Transform firePoint;
    public float alturaDisparoArriba = 1f;
    public float anguloDisparoArriba = 90f;

    [Header("Estado Actual")]
    public PowerUpType poderActual = PowerUpType.None;
    public bool tienePoder => poderActual != PowerUpType.None;

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
        // Si no tiene ningún poder activo, no puede disparar
        if (!tienePoder) return;

        // Apuntado hacia arriba (puedes mantener la misma lógica de salto o cambiarla a otra tecla)
        bool apuntandoArriba = Input.GetButton("Jump" + playerNumber);
        Apuntar(apuntandoArriba);

        // Al presionar el botón de disparo especial, se consume el poder y se dispara 1 sola vez
        if (Input.GetButtonDown("Fire" + playerNumber)) // O el botón que prefieras para disparar
        {
            DispararPoderUnico();
        }
    }

    private void Apuntar(bool arriba)
    {
        if (firePoint == null) return;

        if (arriba)
        {
            firePoint.localPosition = new Vector3(0f, alturaDisparoArriba, 0f);
            firePoint.localRotation = Quaternion.Euler(0f, 0f, anguloDisparoArriba);
        }
        else
        {
            firePoint.localPosition = posicionNormal;
            firePoint.localRotation = rotacionNormal;
        }
    }

    void DispararPoderUnico()
    {
        GameObject prefabAUsar = null;

        // Seleccionar el prefab según el poder que tenga el jugador
        switch (poderActual)
        {
            case PowerUpType.Bala:
                prefabAUsar = prefabBala;
                break;
            case PowerUpType.Cinta:
                prefabAUsar = prefabCinta;
                break;
            case PowerUpType.Tijeras:
                prefabAUsar = prefabTijeras;
                break;
        }

        if (prefabAUsar != null)
        {
            AudioManager.Instance.PlayDisparo();
            GameObject balaObj = Instantiate(prefabAUsar, firePoint.position, firePoint.rotation);
            
            BulletController bala = balaObj.GetComponent<BulletController>();
            if (bala != null)
            {
                bala.playerNumber = playerNumber;
            }
        }

        // ¡CONSUMIR EL PODER! Vuelve a ser "None" (solo espada)
        poderActual = PowerUpType.None;
        
        // Restablecer posición del firePoint por si disparó hacia arriba
        Apuntar(false);
    }

    // Método público para que el Power-Up se lo entregue al jugador
    public void RecogerPowerUp(PowerUpType nuevoPoder)
    {
        poderActual = nuevoPoder;
        Debug.Log("¡Jugador " + playerNumber + " obtuvo el poder: " + nuevoPoder + "!");
    }
}