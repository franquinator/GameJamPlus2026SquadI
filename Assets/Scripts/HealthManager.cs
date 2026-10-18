using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class HealthManager : MonoBehaviour
{
    [Header("Vidas")]
    public int vidasMaximas = 3;
    public int vidas;

    [Header("Corazones (UI)")]
    // GameObjects de los corazones. Se muestran los primeros 'vidas' y se ocultan los demás
    public GameObject[] corazones;

    [Header("Inmunidad")]
    public float segundosInmune = 1.5f;

    [Header("Efecto de daño (parpadeo del alfa)")]
    [Range(0f, 1f)] public float alfaMinimo = 0.25f;
    public float velocidadParpadeo = 12f;

    [Header("Muerte")]
    // Clip suelto: se reproduce con una PlayableGraph, sin necesidad de estados en el Animator
    public AnimationClip animacionMuerte;
    public float segundosMuerte = 1.5f;

    [Header("Respawn")]
    // Si se deja vacío se usa la posición en la que estaba el player al empezar
    public Transform puntoInicio;

    private SpriteRenderer sprite;
    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D col;
    private PlayerController playerController;
    private ShootController shootController;

    private Color colorBase;
    private Vector3 posicionInicio;
    private Quaternion rotacionInicio;

    private Coroutine corrutinaInmunidad;
    private bool inmune;
    private bool muerto;

    // Estado de la reproducción de la animación de muerte
    private PlayableGraph grafoMuerte;
    private RuntimeAnimatorController controllerOriginal;
    private bool reproduciendoMuerte;

    public bool estaMuerto => muerto;
    public bool esInmune => inmune;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        playerController = GetComponent<PlayerController>();
        shootController = GetComponent<ShootController>();

        // Se guarda el color original (el tint del jugador) para no perderlo al tocar el alfa
        colorBase = sprite.color;
    }

    void Start()
    {
        if (puntoInicio != null)
        {
            posicionInicio = puntoInicio.position;
            rotacionInicio = puntoInicio.rotation;
        }
        else
        {
            posicionInicio = transform.position;
            rotacionInicio = transform.rotation;
        }

        vidas = vidasMaximas;
        ActualizarCorazones();
    }

    void OnDisable()
    {
        // Destruye todos los Playables y PlayableOutputs creados por el grafo
        LiberarGrafoMuerte();
    }

    // Punto de entrada del daño. Cada impacto quita una vida entera
    public void RecibirDaño()
    {
        // Si está muerto o inmune el daño se ignora
        if (muerto || inmune) return;

        PerderVida();
    }

    // Resta una vida. Si se acaba, muere; si no, queda inmune unos segundos
    public void PerderVida()
    {
        if (muerto) return;

        vidas = Mathf.Max(0, vidas - 1);
        ActualizarCorazones();

        if (vidas <= 0)
        {
            StartCoroutine(Morir());
        }
        else
        {
            ActivarInmunidad();
        }
    }

    // Muestra los primeros 'vidas' corazones y oculta los que sobren
    private void ActualizarCorazones()
    {
        // Si no hay corazones asignados el script sigue funcionando igual
        if (corazones == null) return;

        for (int i = 0; i < corazones.Length; i++)
        {
            if (corazones[i] == null) continue;

            corazones[i].SetActive(i < vidas);
        }
    }

    private void ActivarInmunidad()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayGolpe();
        }

        if (corrutinaInmunidad != null)
        {
            StopCoroutine(corrutinaInmunidad);
        }

        inmune = true;
        corrutinaInmunidad = StartCoroutine(Parpadear());
    }

    // Efecto de daño: el alfa del sprite parpadea mientras dura la inmunidad
    private IEnumerator Parpadear()
    {
        float transcurrido = 0f;

        while (transcurrido < segundosInmune)
        {
            transcurrido += Time.deltaTime;

            // Oscila entre alfaMinimo y el alfa original
            float t = Mathf.Abs(Mathf.Sin(Time.time * velocidadParpadeo));
            sprite.color = new Color(colorBase.r, colorBase.g, colorBase.b, Mathf.Lerp(alfaMinimo, colorBase.a, t));

            yield return null;
        }

        sprite.color = colorBase;
        inmune = false;
        corrutinaInmunidad = null;
    }

    private IEnumerator Morir()
    {
        muerto = true;
        inmune = false;

        // Si el parpadeo quedó a medias se corta
        if (corrutinaInmunidad != null)
        {
            StopCoroutine(corrutinaInmunidad);
            corrutinaInmunidad = null;
        }

        // Se deja el sprite opaco para que la animación de muerte se vea
        sprite.color = colorBase;

        // Se apaga el control y las colisiones: muerto no se mueve, no dispara y no captura la bandera
        if (playerController != null) playerController.enabled = false;
        if (shootController != null) shootController.enabled = false;
        if (col != null) col.enabled = false;
        if (rb != null) rb.velocity = Vector2.zero;

        ReproducirAnimacionMuerte();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMuerte();
        }

        yield return new WaitForSeconds(segundosMuerte);

        Reaparecer();
    }

    // Reproduce el clip suelto con una PlayableGraph (Patrón de la documentación de Unity)
    private void ReproducirAnimacionMuerte()
    {
        if (animacionMuerte == null || animator == null)
        {
            Debug.LogWarning($"Falta asignar la animación de muerte en {name} (HealthManager).", this);
            return;
        }

        // El grafo y el Animator se pelean si el controller sigue activo, así que se apaga un rato
        controllerOriginal = animator.runtimeAnimatorController;
        animator.runtimeAnimatorController = null;

        grafoMuerte = PlayableGraph.Create("HealthManagerMuerte");
        grafoMuerte.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        AnimationPlayableOutput output = AnimationPlayableOutput.Create(grafoMuerte, "Muerte", animator);
        AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(grafoMuerte, animacionMuerte);
        output.SetSourcePlayable(clipPlayable);

        grafoMuerte.Play();
        reproduciendoMuerte = true;
    }

    private void LiberarGrafoMuerte()
    {
        if (!reproduciendoMuerte) return;

        if (grafoMuerte.IsValid())
        {
            grafoMuerte.Destroy();
        }

        // Se devuelve el controller antes del Rebind para que vuelva al idle
        if (animator != null)
        {
            animator.runtimeAnimatorController = controllerOriginal;
        }

        reproduciendoMuerte = false;
    }

    private void Reaparecer()
    {
        LiberarGrafoMuerte();

        vidas = vidasMaximas;
        ActualizarCorazones();

        transform.position = posicionInicio;
        transform.rotation = rotacionInicio;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        if (col != null) col.enabled = true;

        sprite.color = colorBase;

        // Rebind deja el Animator en su estado inicial (idle) y reinicia los parámetros
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        if (playerController != null) playerController.enabled = true;
        if (shootController != null) shootController.enabled = true;

        muerto = false;
    }
}