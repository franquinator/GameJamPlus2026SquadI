using System.Collections;
using UnityEngine;

public class PowerUpsSpawner : MonoBehaviour
{
    public float frecuenciaEnSeg = 3;
    public int powerUpsEnPantalla = 2;
    public Transform[] spawnPoints;

    public GameObject[] PowerUps;
    public Transform powerUpContainer;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AparecerPowerUp());
    }

    private IEnumerator AparecerPowerUp()
    {
        while (true)
        {
            yield return new WaitForSeconds(frecuenciaEnSeg);
            if (powerUpContainer.childCount < powerUpsEnPantalla)
            {
                GameObject powerUp = PowerUps[Random.Range(0, PowerUps.Length)];
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(powerUp, spawnPoint.position, Quaternion.identity, powerUpContainer);
            }
        }
    }
}
