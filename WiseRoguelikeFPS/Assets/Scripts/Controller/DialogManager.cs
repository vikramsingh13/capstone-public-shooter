using UnityEngine;
using Zenject;
using System;

public class DialogManager : Singleton<DialogManager>
{
    private Level1Manager _level1Manager;

    [Inject]
    public void Construct(Level1Manager level1Manager)
    {
        _level1Manager = level1Manager;
    }


}