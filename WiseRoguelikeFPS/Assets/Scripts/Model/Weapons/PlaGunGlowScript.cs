using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaGunGlowScript : MonoBehaviour
{

    public GameObject PlasmaGun;
    public Weapon PlasmaGunScript;

    [SerializeField] private float BatteryLightBlinkInterval = 0.4f;
    private float BatteryLightBlinkTimer = 0.0f;

    private bool Blink;

    private Color baseColor;
    [SerializeField] private Color RedLightColor;
    [SerializeField] private float MagLightBaseIntensity = 2.0f;
    [SerializeField] private float MagLightBlinkIntensity = 50.0f;

    [SerializeField] private float coilIntensity;

    [SerializeField] private Material CoilMat;
    [SerializeField] private Renderer coilToChange;

    [SerializeField] private Material BatteryMat;
    [SerializeField] private Renderer batteryToChange;
    /*
    void Start()
    {

        PlasmaGunScript = PlasmaGun.GetComponent<Weapon>();
        CoilMat = coilToChange.GetComponent<Renderer>().material;

        BatteryMat = batteryToChange.GetComponent<Renderer>().material;

        baseColor = Color.white;

        CoilMat.EnableKeyword("_EMISSION");
        BatteryMat.EnableKeyword("_EMISSION");

    }

    private void Update()
    {

        //Sets the intensity of the plasma coil's emissive material in relation to the heat of the gun.
        coilIntensity = 0.0f + (PlasmaGunScript.currentHeat / 5);
        CoilMat.SetColor("_EmissionColor", baseColor * coilIntensity);

        //If the weapon is unavailable due to overheating or reloading...
        if(PlasmaGunScript.weaponUnavailable)
        {

            //Start the blink timer.
            BatteryLightBlinkTimer += Time.deltaTime;

            //Turn the blinking light on and off while unavailable.
            if(BatteryLightBlinkTimer >= BatteryLightBlinkInterval)
            {

                Blink = !Blink;
                BatteryLightBlinkTimer = 0.0f;

            }

            //Swaps between the normal green light and the bright red light as Blink alternates between true and false.
            //This is meant to make the light actually blink.
            if(Blink)
            {
                BatteryMat.SetColor("_EmissionColor", RedLightColor * MagLightBlinkIntensity);
            }
            else
            {
                BatteryMat.SetColor("_EmissionColor", baseColor * MagLightBaseIntensity);
            }

        }
        else
        {
            //Sets the light color back to normal when the gun is usable again.
            BatteryMat.SetColor("_EmissionColor", baseColor * MagLightBaseIntensity);
        }

    }*/

}
