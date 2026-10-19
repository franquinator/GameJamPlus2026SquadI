using System.Collections;
using UnityEngine;

// La cinta congela al rival al que le pega: se queda quieto, no puede saltar y
// se le muestra el sprite de cinta por arriba. Sigue recibiendo daño normal.
public class Congelamiento : MonoBehaviour
{
    [Header("Sprite de cinta")]
    // GameObject con un SpriteRenderer que se activa mientras el jugador está congelado
    public GameObject spriteCongelado;

    [Header("Duración")]
    public float duracion = 2f;

    private Coroutine corrutina;

    public bool estaCongelado { get; private set; }

    // Lo llama BulletController cuando la cinta le pega a este jugador
    public void Congelar(float segundos)
    {
        if (segundos <= 0f) return;

        // Si lo vuelven a congelar antes de que se descongele, se reinicia la cuenta
        if (corrutina != null) StopCoroutine(corrutina);
        corrutina = StartCoroutine(CongelarYVolver(segundos));
    }

    // Lo llama HealthManager al morir o reaparecer
    public void QuitarCongelamiento()
    {
        if (corrutina != null)
        {
            StopCoroutine(corrutina);
            corrutina = null;
        }

        estaCongelado = false;
        if (spriteCongelado != null) spriteCongelado.SetActive(false);
    }

    private IEnumerator CongelarYVolver(float segundos)
    {
        estaCongelado = true;
        if (spriteCongelado != null) spriteCongelado.SetActive(true);

        yield return new WaitForSeconds(segundos);

        // Se limpia la referencia antes de llamar para no intentar parar la corrutina actual
        corrutina = null;
        QuitarCongelamiento();
    }
}