using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagController : MonoBehaviour
{
    [SerializeField]
    private Image barraRosa;
    [SerializeField]
    private float cantRosa = 50;
    [SerializeField]
    private float cadaCuantoCambia;
    [SerializeField]
    private float aumento;
    private List<PlayerController> jugadores = new();
    SpriteRenderer colorFlag;
    // Start is called before the first frame update
    void Start()
    {
        colorFlag = GetComponent<SpriteRenderer>();
        StartCoroutine(AumentarBarra());
    }

    // Update is called once per frame
    void Update()
    {


    }
    void OnTriggerEnter2D(Collider2D other)
    {
        // Accedes al tag a través de other.tag o other.gameObject.tag
        if (other.CompareTag("Player"))
        {
            jugadores.Add(other.GetComponent<PlayerController>());
            if(jugadores.Count == 1)
            {
                Color colorTagNew = jugadores[0].GetComponent<SpriteRenderer>().color;
                colorFlag.color = colorTagNew;
            }
            else
            {
                colorFlag.color = Color.white;
            }

        }

    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadores.Remove(collision.GetComponent<PlayerController>());
            if(jugadores.Count == 1)
            {
                Color colorTagNew = jugadores[0].GetComponent<SpriteRenderer>().color;
                colorFlag.color = colorTagNew;
            }
            else
            {
                colorFlag.color = Color.white;
            }
        }
    }
    private IEnumerator AumentarBarra()
    {
        while (true)
        {   
            yield return new WaitUntil(() => jugadores.Count == 1);
            yield return new WaitForSeconds(cadaCuantoCambia);
            if(jugadores.Count == 1)
            {
                if(jugadores[0].playerNumber == 2)
                {
                    cantRosa += aumento;
                }
                else if(jugadores[0].playerNumber == 1)
                {
                    cantRosa -= aumento;
                }

                if (cantRosa >= 100)
                {
                    Debug.Log("gano el rosa");
                }
                else if (cantRosa <= 0)
                {
                    Debug.Log("gano el verde");
                }
                barraRosa.fillAmount = cantRosa / 100f;
            }
        }
        
    }
}

