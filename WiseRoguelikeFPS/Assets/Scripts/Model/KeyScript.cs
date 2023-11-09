using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<PlayerManager>())
        {

            Level1Manager.Instance.KeyGet();
            Destroy(this.gameObject);

        }

    }

}
