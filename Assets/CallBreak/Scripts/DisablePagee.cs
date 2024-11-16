using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisablePagee : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject nextPage;
    [SerializeField] private GameObject disablePage;

    public void PageDisable()
    {
        disablePage.SetActive(false);
    }

    public void MoveNext(bool mode)
    {
        nextPage.SetActive(true);
        if (mode) disablePage.SetActive(false);
    }
     public GameObject panel;
    public void LoadGame()
    {
        if(PlayerPrefs.HasKey("mode")){panel.SetActive(true);PlayerPrefs.DeleteKey("mode");}
        else SceneManager.LoadScene("Game");
    }
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }


    public void GameMode(int va)
    {
         if(va==3)
         {
            PlayerPrefs.SetInt("mode", 3);
         }
    }

    public void DisableEnable()
    {
        if (nextPage.activeInHierarchy)
        {
            nextPage.SetActive(false);
        }
        else
        {
            nextPage.SetActive(true);
        }
    }

}
