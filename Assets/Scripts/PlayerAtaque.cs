using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType { None, Bala, Cinta, Tijeras }

// Controla el ataque del jugador: por defecto ataca con espada y, mientras tenga
// un power-up encima, ese power-up reemplaza a la espada. El poder es temporal:
// cuando se acaba el tiempo el jugador vuelve a la espada solo.
public class PlayerAtaque : MonoBehaviour
{
    public int playerNumber = 1;

    [Header("Punto de salida de los proyectiles")]
    public Transform firePoint;
    public float alturaDisparoArriba = 1f;
    public float anguloDisparoArriba = 90f;

    [Header("Power-ups")]
    // Mientras dura el poder no se puede atacar con la espada
    public GameObject prefabBala;
    public GameObject prefabCinta;
    public GameObject prefabTijeras;
    [Tooltip("Segundos que dura el power-up antes de volver a la espada")]
    public float duracionPoder = 6f;
    [Tooltip("Tiempo mínimo entre dos disparos seguidos del power-up")]
    public float intervaloDisparo = 0.35f;
    public float velocidadProyectil = 18f;

    [Header("Espada (ataque por defecto)")]
    // Barrido corto por delante del personaje
    public Vector2 tamanoEspada = new Vector2(1.8f, 1.3f);
    public float alcanceEspada = 0.9f;
    public float alturaEspada = 0.1f;
    public float cooldownEspada = 0.4f;

    [Header("Animación")]
    public string triggerAtaque = "atacar";
    public string triggerDisparo = "disparar";
    public string paramConPoder = "conPoder";

    private Animator animator;
    private readonly HashSet<string> parametrosAnimator = new HashSet<string>();

    private Vector3 posicionNormal;
    private Quaternion rotacionNormal;

    private float siguienteEspada;
    private float siguienteDisparo;
    private float finPoder;

    // Poder activo. None significa que el jugador solo tiene espada
    public PowerUpType poderActual { get; private set; } = PowerUpType.None;

    public bool tienePoder => poderActual != PowerUpType.None;

    // Segundos que le quedan al poder antes de expirar (0 si no hay poder)
    public float tiempoPoder => tienePoder ? Mathf.Max(0f, finPoder - Time.time) : 0f;

    void Awake()
    {
        animator = GetComponent<Animator>();

        // Se anotan los parámetros que existen para no llamar a los que falten
        // (SetTrigger con un parámetro inexistente ensucia la consola)
        if (animator != null)
        {
            foreach (AnimatorControllerParameter parametro in animator.parameters)
            {
                parametrosAnimator.Add(parametro.name);
            }
        }

        if (firePoint != null)
        {
            posicionNormal = firePoint.localPosition;
            rotacionNormal = firePoint.localRotation;
        }
    }

    void Update()
    {
        // El poder es temporal: al cumplirse el tiempo vuelve la espada
        if (tienePoder && Time.time >= finPoder)
        {
            TerminarPoder();
        }

        // Mantener la tecla de salto también sirve para apuntar los proyectiles hacia arriba
        Apuntar(Input.GetButton("Jump" + playerNumber));

        if (Input.GetButtonDown("Fire" + playerNumber))
        {
            if (tienePoder) UsarPoder();
            else AtacarConEspada();
        }

        ReflejarPoderEnAnimator();
    }

    private void Apuntar(bool arriba)
    {
        if (firePoint == null) return;

        if (arriba)
        {
            // Se sube el punto de salida para que el proyectil no nazca al costado del personaje
            firePoint.localPosition = new Vector3(0f, alturaDisparoArriba, 0f);
            firePoint.localRotation = Quaternion.Euler(0f, 0f, anguloDisparoArriba);
        }
        else
        {
            firePoint.localPosition = posicionNormal;
            firePoint.localRotation = rotacionNormal;
        }
    }

    // Ataque por defecto: un barrido corto por delante del jugador
    private void AtacarConEspada()
    {
        if (Time.time < siguienteEspada) return;
        siguienteEspada = Time.time + cooldownEspada;

        DispararTrigger(triggerAtaque);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayEspada();

        float lado = MiraDerecha() ? 1f : -1f;
        Vector2 centro = (Vector2)transform.position + new Vector2(lado * alcanceEspada, alturaEspada);

        Collider2D[] alcanzados = Physics2D.OverlapBoxAll(centro, tamanoEspada, 0f);
        foreach (Collider2D colision in alcanzados)
        {
            if (colision == null || !colision.CompareTag("Player")) continue;

            HealthManager salud = colision.GetComponent<HealthManager>();
            if (salud == null || salud.estaMuerto) continue;

            // Solo daña al rival, nunca a quien está atacando
            PlayerController rival = colision.GetComponent<PlayerController>();
            if (rival == null || rival.playerNumber == playerNumber) continue;

            salud.RecibirDaño();
        }
    }

    private void UsarPoder()
    {
        if (Time.time < siguienteDisparo) return;

        GameObject prefab = GetPrefab(poderActual);
        if (prefab == null || firePoint == null) return;

        siguienteDisparo = Time.time + intervaloDisparo;

        DispararTrigger(triggerDisparo);
        if (AudioManager.Instance != null) AudioManager.Instance.PlayDisparo();

        GameObject proyectil = Instantiate(prefab, firePoint.position, firePoint.rotation);

        // El prefab del proyectil ya viene con su BulletController; solo se le
        // dice de quién es y qué power-up representa
        BulletController bala = proyectil.GetComponent<BulletController>();
        if (bala == null) bala = proyectil.AddComponent<BulletController>();
        bala.Lanzar(playerNumber, poderActual, velocidadProyectil);
    }

    private GameObject GetPrefab(PowerUpType tipo)
    {
        switch (tipo)
        {
            case PowerUpType.Bala: return prefabBala;
            case PowerUpType.Cinta: return prefabCinta;
            case PowerUpType.Tijeras: return prefabTijeras;
            default: return null;
        }
    }

    // Lo llama el PowerUpPickup cuando el jugador toca un power-up
    public void RecogerPowerUp(PowerUpType nuevoPoder)
    {
        if (nuevoPoder == PowerUpType.None) return;

        TerminarPoder();

        poderActual = nuevoPoder;
        finPoder = Time.time + duracionPoder;
        siguienteDisparo = 0f;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayPowerUp();

        Debug.Log("Jugador " + playerNumber + " agarró el power-up: " + nuevoPoder + " (" + duracionPoder + "s)");
    }

    // Quita el power-up de golpe, por ejemplo al morir o reaparecer
    public void QuitarPoder()
    {
        TerminarPoder();
        Apuntar(false);
    }

    private void TerminarPoder()
    {
        if (poderActual == PowerUpType.None) return;

        poderActual = PowerUpType.None;
        Debug.Log("Se venció el power-up del jugador " + playerNumber + ": vuelve la espada");
    }

    // El PlayerController voltea el transform, así que el lado se lee de ahí
    private bool MiraDerecha()
    {
        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null) return controller.MiraDerecha;

        return Mathf.Cos(transform.eulerAngles.y * Mathf.Deg2Rad) >= 0f;
    }

    private void DispararTrigger(string parametro)
    {
        if (animator != null && parametrosAnimator.Contains(parametro))
        {
            animator.SetTrigger(parametro);
        }
    }

    private void ReflejarPoderEnAnimator()
    {
        if (animator != null && parametrosAnimator.Contains(paramConPoder))
        {
            animator.SetBool(paramConPoder, tienePoder);
        }
    }
}