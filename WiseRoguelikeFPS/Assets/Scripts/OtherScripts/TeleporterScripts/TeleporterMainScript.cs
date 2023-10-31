using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterMainScript : MonoBehaviour
{

    [SerializeField] private GameObject StartSequenceTrigger;
    [SerializeField] private GameObject EndLevelTrigger;

    public void EnableSequenceStartTrigger()
    {

        StartSequenceTrigger.SetActive(true);

    }

    public void StartEndSquence() { 
    
        StartSequenceTrigger.SetActive(false);
        Level1Manager.Instance.StartExfiltration();
    
    }

    public void EnableEndTrigger() { 
    
        EndLevelTrigger.SetActive(true);
    
    }

    public void DisableEndTrigger()
    {

        Level1Manager.Instance.GameWin();
        EndLevelTrigger.SetActive(false);

    }

}
