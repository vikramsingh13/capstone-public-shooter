using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitLevelScript : MonoBehaviour
{

    [SerializeField] private GameObject Teleporter;

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<Player>())
        {

            Teleporter.GetComponent<TeleporterMainScript>().DisableEndTrigger();

        }

    }

}
