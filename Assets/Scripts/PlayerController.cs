using UnityEngine;

public class PlayerController : MonoBehaviour
{
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
    [SerializeField]
    private int playerNumber = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Leer entrada horizontal (A/D o flechas izquierda/derecha)
        movimientoHorizontal = Input.GetAxisRaw("Horizontal"+playerNumber) * velocidad;

        // Verificar si presionó saltar (Espacio)
        if (Input.GetButtonDown("Jump"+playerNumber) && enSuelo)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
        }

        // Voltear sprite según la dirección
        GestionarOrientacion();
    }

    void FixedUpdate()
    {
        // Comprobar si está tocando el suelo
        enSuelo = Physics2D.OverlapCircle(controladorSuelo.position, radioSuelo, queEsSuelo);

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
        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
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