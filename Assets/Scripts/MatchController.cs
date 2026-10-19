using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PausarJuego();
    }
    public void PausarJuego()
    {
        Time.timeScale = 0;
        AudioManager.Instance.StopMusic();
    }
    public void ContinuarJuego()
    {
        Time.timeScale = 1;
        AudioManager.Instance.PlayMatchMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
