using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticData : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Set the GameObjects of the type of bullet hole left on whatever is shot, then in the script connect it to a string tag such that an object with that tag shot will have that kind of bullet hole \n\n0: 'MetalBulletImpact'  ->  'MetalImpactObject'\n1: 'WaterBulletImpact'  ->  'WaterImpactObject'\n2: 'FleshBulletImpact'  ->  'FleshImpactObject'")]
    public GameObject[] particleObjects;

    public static Dictionary<string, GameObject> particleDictionary;

    private void Start()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        particleDictionary = new Dictionary<string, GameObject>()
        {
            //{ "MetalBulletImpact", particleObjects[0] },
            //{ "WaterBulletImpact", particleObjects[1] }
        };
    }
}
