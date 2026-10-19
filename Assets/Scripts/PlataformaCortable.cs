using System.Collections;
using UnityEngine;

// Hace desaparecer una plataforma unos segundos y después la vuelve a mostrar.
// Lo usan las tijeras. El componente se agrega en runtime al GameObject que toca
// el proyectil, así que no hay que configurarlo en el editor.
public class PlataformaCortable : MonoBehaviour
{
    private Coroutine corrutina;

    // Atajo estático: corta el GameObject recibido sin necesidad de que tenga el componente
    public static void Cortar(GameObject objetivo, float segundos)
    {
        if (objetivo == null || segundos <= 0f) return;

        PlataformaCortable cortable = objetivo.GetComponent<PlataformaCortable>();
        if (cortable == null) cortable = objetivo.AddComponent<PlataformaCortable>();

        cortable.Cortar(segundos);
    }

    public void Cortar(float segundos)
    {
        // Si la cortan otra vez antes de volver, se reinicia la cuenta regresiva
        if (corrutina != null) StopCoroutine(corrutina);
        corrutina = StartCoroutine(CortarYVolver(segundos));
    }

    private IEnumerator CortarYVolver(float segundos)
    {
        Collider2D[] colisiones = GetComponentsInChildren<Collider2D>(true);
        SpriteRenderer[] imagenes = GetComponentsInChildren<SpriteRenderer>(true);

        foreach (Collider2D colision in colisiones)
        {
            if (colision != null) colision.enabled = false;
        }
        foreach (SpriteRenderer imagen in imagenes)
        {
            if (imagen != null) imagen.enabled = false;
        }

        yield return new WaitForSeconds(segundos);

        foreach (Collider2D colision in colisiones)
        {
            if (colision != null) colision.enabled = true;
        }
        foreach (SpriteRenderer imagen in imagenes)
        {
            if (imagen != null) imagen.enabled = true;
        }

        corrutina = null;

        // Ya no hace falta el componente hasta la próxima tijera
        Destroy(this);
    }
}