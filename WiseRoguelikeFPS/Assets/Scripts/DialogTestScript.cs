using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTestScript : MonoBehaviour
{

    private float testTimer;
    private bool hasSpoken;

    private void Update()
    {

        testTimer += Time.deltaTime;

        if(testTimer >= 5 && hasSpoken == false)
        {
            DialogManager.Instance.NewAnnouncement("Oof!", "Sorry there!");

            hasSpoken = true;

        }

    }

}
