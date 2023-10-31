using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Manager : MonoBehaviour
{

    public static Level1Manager Instance;

    [SerializeField] private int KeysCollected;
    [SerializeField] private int TotalKeys;

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

    // Start is called before the first frame update
    void Start()
    {

        if (Instance == null)
        {

            Instance = this;

        }
        else
        {

            Destroy(this.gameObject);

        }

    }

    // Update is called once per frame
    void Update()
    {
        
        if(EndgameToggle == true)
        {

            EndCountdown -= Time.deltaTime;

        }

        if(Player.GetComponent<PlayerStats>().HitPoints <= 0)
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

        }

    }

    public void StartExfiltration()
    {

        if(KeysCollected == TotalKeys)
        {

            EndgameToggle = true;

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

    public void KeyGet()
    {

        KeysCollected++;

        if(KeysCollected > TotalKeys)
        {

            KeysCollected = TotalKeys;

        }

    }

}
