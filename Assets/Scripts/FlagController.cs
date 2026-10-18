using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FlagController : MonoBehaviour
{
    [SerializeField]
    private Image barraRosa;
    [SerializeField]
    private GameObject cartelRosa;
    [SerializeField]
    private GameObject cartelVerde;
    [SerializeField]
    private float cantRosa = 50;
    [SerializeField]
    private float cadaCuantoCambia;
    [SerializeField]
    private float aumento;
    private List<PlayerController> jugadores = new();
    SpriteRenderer colorFlag;

    [SerializeField] private Transform[] posicionesBandera;
    [SerializeField] private float tiempoEntrePosiciones = 10f;
    private int numAleatorio;
    [SerializeField] private Sprite banderaBase;
    [SerializeField] private Sprite banderaPlayer1;
    [SerializeField] private Sprite banderaPlayer2;

    void Start()
    {
        colorFlag = GetComponent<SpriteRenderer>();
        StartCoroutine(AumentarBarra());
        numAleatorio = Random.Range(0, posicionesBandera.Length);
        StartCoroutine(moverBandera());
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
            if (jugadores.Count == 1 && jugadores[0].playerNumber == 2)
            {
                colorFlag.sprite = banderaPlayer2;
            }
            else if (jugadores.Count == 1 && jugadores[0].playerNumber == 1)
            {
                colorFlag.sprite = banderaPlayer1;
            }
            else
            {
                colorFlag.sprite = banderaBase;
            }

        }

    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadores.Remove(collision.GetComponent<PlayerController>());
            if (jugadores.Count == 1)
            {
                if (jugadores[0].playerNumber == 2)
                {
                    colorFlag.sprite = banderaPlayer2;
                }
                else if (jugadores[0].playerNumber == 1)
                {
                    colorFlag.sprite = banderaPlayer1;
                }
            }
            else
            {
                colorFlag.sprite = banderaBase;
            }
        }
    }
    private IEnumerator AumentarBarra()
    {
        while (true)
        {
            yield return new WaitUntil(() => jugadores.Count == 1);
            yield return new WaitForSeconds(cadaCuantoCambia);
            if (jugadores.Count == 1)
            {
                AudioManager.Instance.PlayFlagPoint();
                if (jugadores[0].playerNumber == 2)
                {
                    cantRosa += aumento;
                }
                else if (jugadores[0].playerNumber == 1)
                {
                    cantRosa -= aumento;
                }

                if (cantRosa >= 100)
                {
                    Debug.Log("gano el rosa");
                    StartCoroutine(Win(2));
                }
                else if (cantRosa <= 0)
                {
                    Debug.Log("gano el verde");
                    StartCoroutine(Win(1));
                }
                barraRosa.fillAmount = cantRosa / 100f;
            }
        }

    }
    private IEnumerator Win(int playerNumber)
    {
        AudioManager.Instance.StopMusic();
        AudioManager.Instance.PlayWin();
        if (playerNumber == 2)
        {
            cartelRosa.SetActive(true);
        }
        else if (playerNumber == 1)
        {
            cartelVerde.SetActive(true);
        }
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(2);
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator moverBandera()
    {
        while (true)
        {
            yield return new WaitForSeconds(tiempoEntrePosiciones);
            transform.position = posicionesBandera[numAleatorio].position;
            numAleatorio = Random.Range(0, posicionesBandera.Length);
        }
    }
}

