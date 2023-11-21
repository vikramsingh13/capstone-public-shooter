using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Level1Manager : Singleton<Level1Manager>
{
    [SerializeField] private int _totalLevelObjectiveCompleted;
    [SerializeField] private int _totalLevelObjectiveNeeded;

    private bool _allLevelObjectivesCompleted = false;
    public bool AllLevelObjectivesCompleted
    {
        get
        {
            return _allLevelObjectivesCompleted;
        }
    }

    private bool EndgameToggle = false;
    public bool ReadyToEnd = false;
    public float EndCountdown;

    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject LevelEnd;

    public AudioClip StandardMusic;
    public AudioClip EndgameMusic;

    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject gameUIPanel;

    private AudioManager _audioManager;

    [Inject]
    public void Construct(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }

    // Start is called before the first frame update
    void Start()
    {
        //TODO REFACTOR ALL MANUAL ASSIGN 
        //USE PROGRAMMATICAL APPROACH
        //TODO: refactor to use events
        _audioManager.StartMusic(StandardMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
        if(EndgameToggle == true)
        {
            EndCountdown -= Time.deltaTime;
        }

        /*if(Player.GetComponent<PlayerStats>().HitPoints <= 0)
        {

            GameLose();

        }

        if(KeysCollected == TotalKeys && EndgameToggle == false)
        {

            LevelEnd.gameObject.GetComponent<TeleporterMainScript>().EnableSequenceStartTrigger();

        }

        if(EndCountdown <= 0)
        {

            LevelEnd.gameObject.GetComponent<TeleporterMainScript>().EnableEndTrigger();

        }*/

    }

    public void StartExfiltration()
    {
        if(_totalLevelObjectiveCompleted == _totalLevelObjectiveNeeded)
        {
            EndgameToggle = true;
            //TODO REFACTOR ALL MANUAL ASSIGN 
            //USE PROGRAMMATICAL APPROACH
            _audioManager.StartMusic(EndgameMusic);
        }
    }

    public void ExfiltrationCheck()
    {
        if(EndCountdown <= 0 && EndgameToggle == true)
        {
            GameWin();
        }
    }

    public void GameWin()
    {
        gameUIPanel.SetActive(false);
        winPanel.SetActive(true);
    }

    public void GameLose()
    {
        gameUIPanel.SetActive(false);
        losePanel.SetActive(true);
    }

    public void FinishLevelObjective()
    {
        _totalLevelObjectiveCompleted++;
        if(_totalLevelObjectiveCompleted >= _totalLevelObjectiveNeeded)
        {
            _allLevelObjectivesCompleted = true;
            _totalLevelObjectiveCompleted = _totalLevelObjectiveNeeded;
        }
    }

}
