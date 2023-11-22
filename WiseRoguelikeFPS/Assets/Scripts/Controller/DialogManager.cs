using UnityEngine;
using Zenject;
using System;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class DialogManager : Singleton<DialogManager>
{
    private Level1Manager _level1Manager;

    private GameObject DialogBox;
    private TextMeshProUGUI HeaderText;
    private TextMeshProUGUI BodyText;

    private float DialogCountdown = 5.0f;

    [Inject]
    public void Construct(Level1Manager level1Manager)
    {
        //_level1Manager = level1Manager;
    }

    private void Start()
    {

        //For the videoAssignment: the name of the scene is Menu or LoadoutMenu then skip update
        //TODO: refactor to use events for game state changes
        if (SceneManager.GetActiveScene().name == "Menu" || SceneManager.GetActiveScene().name == "LoadoutMenu")
        {
            return;
        }

        DialogBox = GameObject.Find("DialogBoxAssembly");
        HeaderText = GameObject.Find("DialogHeaderText").GetComponent<TextMeshProUGUI>();
        BodyText = GameObject.Find("DialogBodyText").GetComponent<TextMeshProUGUI>();
        DialogBox.SetActive(false);
        Debug.Log("The dialog manager is live!");

        //NewAnnouncement("Hello there!", "We are so back!");

    }

    public void NewAnnouncement(string headerText, string bodyText)
    {

        StartCoroutine(AnnouncementEnum(headerText, bodyText));

    }

    private IEnumerator AnnouncementEnum(string h, string t)
    {

        DialogBox.SetActive(true);
        HeaderText.text = h;
        BodyText.text = t;
        yield return new WaitForSeconds(DialogCountdown);
        HeaderText.text = "";
        BodyText.text = "";
        DialogBox.SetActive(false);

    }

}