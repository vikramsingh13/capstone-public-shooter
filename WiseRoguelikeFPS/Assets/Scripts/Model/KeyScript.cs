using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class KeyScript : MonoBehaviour
{
    private Level1Manager _level1Manager;

    [Inject]
    public void Construct(Level1Manager level1Manager)
    {
        _level1Manager = level1Manager;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.GetComponent<Player>())
        {
            _level1Manager.FinishLevelObjective();
            Destroy(this.gameObject);
        }

    }

}
