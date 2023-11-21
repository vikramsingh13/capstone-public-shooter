using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StartEndSequenceScript : MonoBehaviour
{

    [SerializeField] private GameObject Teleporter;

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<Player>())
        {

            Teleporter.GetComponent<TeleporterMainScript>().StartEndSquence();

        }     

    }

}
