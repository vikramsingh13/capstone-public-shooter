using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerResetScript : MonoBehaviour
{

    [SerializeField] private Transform PlayerResetLocation;
    [SerializeField] private float DamageOnReset;


    private void OnTriggerStay(Collider collision)
    {

        Debug.Log("Something hit me!");
        Debug.Log(collision.gameObject.tag.ToString());
        Debug.Log(collision.gameObject.GetComponent<PlayerManager>());

        if (collision.gameObject.GetComponent<PlayerManager>())
        {

            Debug.Log("Resetting player.");
            Debug.Log("Going to X:" + PlayerResetLocation.position.x + ", Y:" + PlayerResetLocation.position.y + ", Z:" + PlayerResetLocation.position.z);

            collision.gameObject.SetActive(false);

            collision.GetComponent<Transform>().position = new Vector3(PlayerResetLocation.position.x, PlayerResetLocation.position.y, PlayerResetLocation.position.z);

            collision.gameObject.SetActive(true);

        }

    }

}
