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
    //for demo state tracking only
    private bool _sentStartDialog = false;
    private bool _sentEndDialog = false;
    private bool _sentBossDialog = false;
    private int _currentLevelObjectiveCompleted = 0;

    private float DialogCountdown = 5.0f;

    [Inject]
    public void Construct(Level1Manager level1Manager)
    {
        _level1Manager = level1Manager;
    }

    private void Start()
    {

        //For the videoAssignment: the name of the scene is Menu or LoadoutMenu then skip update
        //TODO: refactor to use events for game state changes
        if (SceneManager.GetActiveScene().name != "TestFirstLevelScene")
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

    private void Update()
    {
        //demo announcements
        if(_level1Manager.CurrentSceneName == "TestFirstLevelScene" && _level1Manager.CurrentSceneName != _level1Manager.PreviousSceneName && !_sentStartDialog)
        {
            if(DialogBox == null)
            {
                DialogBox = GameObject.Find("DialogBoxAssembly");
            }
            if(HeaderText == null)
            {
                HeaderText = GameObject.Find("DialogHeaderText").GetComponent<TextMeshProUGUI>();
            }
            if (BodyText == null)
            {
                BodyText = GameObject.Find("DialogHeaderText").GetComponent<TextMeshProUGUI>();
            }
            DialogBox.SetActive(false);
            Debug.Log("The dialog manager is live!");

            NewAnnouncement("!WARNING!", "UNKNOWN FORMS DETECTED! PROCEED WITH CAUTION!");
            _sentStartDialog = true;
        }

        //demo announcements
        if(_level1Manager.TotalLevelObjectiveCompleted > _currentLevelObjectiveCompleted)
        {
            _currentLevelObjectiveCompleted = _level1Manager.TotalLevelObjectiveCompleted;

            NewAnnouncement("Notice!", $"You have collected {_currentLevelObjectiveCompleted} out of {_level1Manager.TotalLevelObjectiveNeeded}");
        }

        //demo announcements
        if (_level1Manager.EndgameSequenceStarted && !_sentEndDialog)
        {
            NewAnnouncement("!ATTENTION!", $"READING MULTIPLE ENERGY SPIKES ACROSS THE PLANET! THAT STRUCTURE MAY BE CONNECTED.");
            _sentEndDialog = true;
        }
    }



    public void NewAnnouncement(string headerText, string bodyText)
    {

        StartCoroutine(AnnouncementEnum(headerText, bodyText));

    }

    //fills up the dialog for the given time and then removes the text and hides the dialog box
    private IEnumerator AnnouncementEnum(string h, string t)
    {

        DialogBox.SetActive(true);
        //for demo
        HeaderText.text = "";
        BodyText.text = "";
        //
        HeaderText.text = h;
        BodyText.text = t;
        yield return new WaitForSeconds(DialogCountdown);
        HeaderText.text = "";
        BodyText.text = "";
        DialogBox.SetActive(false);

    }

}