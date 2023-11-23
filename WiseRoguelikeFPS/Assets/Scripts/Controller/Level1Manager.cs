using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class Level1Manager : Singleton<Level1Manager>
{
    [SerializeField] private int _totalLevelObjectiveCompleted = 0;
    public virtual int TotalLevelObjectiveCompleted
    {
        get
        {
            return _totalLevelObjectiveCompleted;
        }
    }
    [SerializeField] private int _totalLevelObjectiveNeeded = 3;
    public virtual int TotalLevelObjectiveNeeded
    {
        get
        {
            return _totalLevelObjectiveNeeded;
        }
    }

    private bool _allLevelObjectivesCompleted = false;
    public virtual bool AllLevelObjectivesCompleted
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

    [SerializeField]
    private string StandardMusic = "Assets/Audio/BackgroundMusic.mp3";
    [SerializeField]
    private string EndgameMusic = "Assets/Audio/IntoTheUnknown Ending Edit.mp3";

    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject gameUIPanel;

    private AudioManager _audioManager;
    private bool _backgroundMusicStarted = false;

    //scene tracking for demo
    private string _currentSceneName = "";
    public virtual string CurrentSceneName
    {
        get
        {
            return _currentSceneName;
        }
    }
    private string _previousSceneName = "";
    public virtual string PreviousSceneName
    {
        get
        {
            return _previousSceneName;
        }
    }

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
        //_audioManager.StartMusic(StandardMusic);
    }

    // Update is called once per frame
    void Update()
    {
        //scene tracking for demo
        if(SceneManager.GetActiveScene().name != _currentSceneName)
        {
            _previousSceneName = _currentSceneName;
            _currentSceneName = SceneManager.GetActiveScene().name;
        }

        /*if(_currentSceneName == "TestFirstLevelScene" && !_backgroundMusicStarted)
        {
            _audioManager.StartMusic(StandardMusic);
            _backgroundMusicStarted = true;
        }*/
        /*if(EndgameToggle == true)
        {
            EndCountdown -= Time.deltaTime;
        }

        if(Player.GetComponent<Player>()._currentPlayerHealth <= 0)
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
            Debug.Log("This is the endgame music: " + EndgameMusic);
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
