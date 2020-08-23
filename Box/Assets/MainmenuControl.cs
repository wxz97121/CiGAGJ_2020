using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainmenuControl : MonoBehaviour
{
    public GameObject BGM;
    public Image BG;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartFade());
        DontDestroyOnLoad(BGM);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    IEnumerator StartFade()
    {
        yield return new WaitForSeconds(2f);
        BG.DOFade(0, 4f);
    }

}
