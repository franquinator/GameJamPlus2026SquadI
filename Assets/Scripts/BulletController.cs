using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float velocidad = 20f;
    public int daño = 1;

    [Header("Dueño")]
    // PlayerAtaque lo copia al instanciar el proyectil, así sabe a quién no debe dañar
    public int playerNumber = 1;

    [Header("Tipo de power-up que representa")]
    public PowerUpType tipo = PowerUpType.Bala;

    [Header("Tijeras")]
    [Tooltip("Segundos que la plataforma desaparece después de ser cortada")]
    public float duracionCorte = 2f;

    [Header("Cinta")]
    [Tooltip("Segundos que el rival queda congelado tras recibir la cinta")]
    public float duracionCongelamiento = 2f;

    private Rigidbody2D cuerpo;

    void Awake()
    {
        cuerpo = GetComponent<Rigidbody2D>();
    }

    // Lo llama PlayerAtaque al instanciar el proyectil.
    // El Rigidbody2D sin gravedad hace falta porque las plataformas del nivel no
    // tienen cuerpo propio: sin él las tijeras nunca chocarían contra ellas.
    public void Lanzar(int dueno, PowerUpType nuevoTipo, float nuevaVelocidad)
    {
        playerNumber = dueno;
        tipo = nuevoTipo;
        velocidad = nuevaVelocidad;

        if (cuerpo == null) cuerpo = gameObject.AddComponent<Rigidbody2D>();
        cuerpo.gravityScale = 0f;
        cuerpo.velocity = transform.right * nuevaVelocidad;
    }

    void Update()
    {
        // Si no hay Rigidbody2D (por ejemplo si se arrastra uno al editor a mano)
        // el proyectil se mueve con su propio transform
        if (cuerpo == null)
        {
            transform.Translate(transform.right * velocidad * Time.deltaTime, Space.World);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController objetivo = other.GetComponent<PlayerController>();

            // Solo daña al otro jugador. Si le pega a quien la disparó no le hace daño y sigue de largo
            if (objetivo != null && objetivo.playerNumber != playerNumber)
            {
                // La cinta congela al rival al que le pega. Lo congela igual aunque
                // el golpe no le haga daño: el poder no depende del daño
                if (tipo == PowerUpType.Cinta)
                {
                    Congelamiento congelado = other.GetComponent<Congelamiento>();
                    if (congelado != null) congelado.Congelar(duracionCongelamiento);
                }

                HealthManager salud = other.GetComponent<HealthManager>();
                if (salud != null)
                {
                    salud.RecibirDaño();
                    Destroy(gameObject);
                }
            }

            return;
        }

        // Las tijeras se clavan en lo que tocan y lo hacen desaparecer un rato
        if (tipo == PowerUpType.Tijeras)
        {
            PlataformaCortable.Cortar(other.gameObject, duracionCorte);
        }

        Destroy(gameObject);
    }
}