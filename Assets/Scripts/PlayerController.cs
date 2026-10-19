using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    [Header("Movimiento")]
    public float velocidad = 5f;
    private float movimientoHorizontal = 0f;

    [Header("Salto")]
    public float fuerzaSalto = 10f;
    private bool enSuelo = false;
    public Transform controladorSuelo;
    public LayerMask queEsSuelo;
    public float radioSuelo = 0.2f;

    private Rigidbody2D rb;
    private bool mirandoDerecha = true;
    public int playerNumber = 1;

    private Congelamiento congelamiento;

    // Lo consultan otros scripts (por ejemplo PlayerAtaque) para saber hacia
    // dónde mira el personaje, ya que el salto se aplica girando el transform
    public bool MiraDerecha => mirandoDerecha;

    // La cinta congela al jugador: mientras lo está no se mueve ni salta
    private bool EstaCongelado => congelamiento != null && congelamiento.estaCongelado;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        congelamiento = GetComponent<Congelamiento>();
    }

    void Update()
    {
        if (Time.timeScale <= 0)return;
        // MOVIMIENTO
        // Leer entrada horizontal (A/D o flechas izquierda/derecha)
        movimientoHorizontal = Input.GetAxisRaw("Horizontal" + playerNumber) * velocidad;

        // ANIMACIONES
        // Verificar si presionó saltar (Espacio)

        if (Input.GetButtonDown("Jump" + playerNumber) && enSuelo && !EstaCongelado)
        {
            animator.SetTrigger("jump");
            AudioManager.Instance.PlaySalto();
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
        }
        animator.SetBool("enSuelo", enSuelo);
        animator.SetBool("caminando", rb.velocity.x > 0.1 || rb.velocity.x < -0.1);
        // Voltear sprite según la dirección
        GestionarOrientacion();
    }

    void FixedUpdate()
    {
        // Comprobar si está tocando el suelo
        enSuelo = Physics2D.OverlapCircle(controladorSuelo.position, radioSuelo, queEsSuelo);

        // Congelado: se frena en seco y se queda quieto en el aire
        if (EstaCongelado)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        // Aplicar movimiento horizontal
        rb.velocity = new Vector2(movimientoHorizontal, rb.velocity.y);
    }

    private void GestionarOrientacion()
    {
        
        if (movimientoHorizontal > 0 && !mirandoDerecha)
        {
            Girar();
        }
        else if (movimientoHorizontal < 0 && mirandoDerecha)
        {
            Girar();
        }
    }

    private void Girar()
    {
        mirandoDerecha = !mirandoDerecha;
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }

    // Dibujar el radio del suelo en el editor de Unity (Gizmos)
    private void OnDrawGizmos()
    {
        if (controladorSuelo != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(controladorSuelo.position, radioSuelo);
        }
    }
}