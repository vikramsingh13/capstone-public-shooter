using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadoutMenu : MonoBehaviour
{
    public void Start()
    {
        //selection arrows for both frames
        GameObject.Find("GunSelectionArrowButton").GetComponent<Button>().onClick.AddListener(StartGame);
        GameObject.Find("ActiveLoadoutArrowButton").GetComponent<Button>().onClick.AddListener(StartGame);

    }

    private void StartGame()
    {
        SceneManager.LoadScene("TestFirstLevelScene");
    }
}