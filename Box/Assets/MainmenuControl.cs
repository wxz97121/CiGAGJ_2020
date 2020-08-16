using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainmenuControl : MonoBehaviour
{
    public GameObject BGM;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(BGM);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
