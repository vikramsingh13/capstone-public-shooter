using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;

public class GunController : MonoBehaviour
{
    //Public Inspector Variables-----------------------------------------------------------------------------
    //Specifications for the GunController
    [Space(4)]
    [Header("GunController Specifications")]

    [Tooltip("True means the gun will fire as long as the fire button is held down")]
    public bool automaticFiring;

    [Tooltip("Sets the amount of bullets fired per second, Therefore the value added is = 1000/fireRate")]
    public float fireRate = 5;

    [Space(2)]
    [Header("   Burst-Fire Specifications")]
    [Tooltip("True means the gun will burst fire as long as the fire button is held down\n\nfullyAutomaticFiring will be set to true in this case")]
    public bool burstFiring;

    [Tooltip("This is the time in MILLISECONDS between bursts. Only takes effect if burstFiring is enabled")]
    public float burstFireDelay = 200;

    [Tooltip("This sets the amount of bullets that will be fired per burst")]
    public int bulletsPerBurst = 5;

    [Space(4)]
    [Header("Ammo/Overheat Specifications")]

    [Tooltip("Set the max heat value before the weapon overheats")]
    public float maxHeat = 100f;

    [Tooltip("Set the amount of heat added to the currentHeat value per bullet")]
    public float heatPerShot = 1f;

    [Tooltip("Set the amount of heat deducted per second when you stop shooting for a short while")]
    public float coolDownPerSecond = 5f;

    [Tooltip("Set the amount of time in MILLISECONDS you must wait if the weapon overheats")]
    public float overheatCooldown = 5000f;

    [Tooltip("Set the amount of time in MILLISECONDS you must wait when you reload")]
    public float manualCooldown = 2000f;

    //Required Attributes for the GunController

    [Space(4)]
    [Header("Required Gun Attributes to function")]

    [Tooltip("Required for raycasting and bullet accuracy")]
    public Transform myCameraHead;

    [Space(1)]
    [Header("   Bullet Attributes")]

    [Tooltip("Set the bullet GameObject here, a prefab of any bullet object will work so long as it has a RigidBody")]
    public GameObject bullet;

    [Tooltip("Set this to the transform of a GameObject to decide where the bullets will spawn when instantiated\n\nThis also determins where the muzzle flash is drawn")]
    public Transform firePosition;

    [Tooltip("Set the muzzflash particle effect GameObject")]
    public GameObject muzzleFlash;

    //[Tooltip("Set a reference to the staticVariable library, to access particle effects")]
    //public GameObject BulletImpactStaticData;
    //private StaticData staticData;

    //Private Member Variables-----------------------------------------------------------------------------
    private bool shooting, readyToShoot = true, reload = false, singleShot=false;
    private int currentBulletsShot = 0;
    private float currentHeat;


    // Start is called before the first frame update
    void Start()
    {
        if(burstFiring)
        {
            automaticFiring = true;
        }

        StartCoroutine(PassiveCooldown());
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
        GunManager();
    }

    //Method to handle processes other than "Shooting"
    private void GunManager()
    {
        //If player reloads, start the coroutine
        if (Input.GetKeyDown(KeyCode.R) && !(currentHeat <= 0))
        {
            reload = true;
            StartCoroutine(ReloadCoroutine());
        }
    }

    //Reload method, called from the coroutine sets the heat back to 0 and resets the burst fire bullet count
    private void Reload()
    {
        currentHeat = 0;
        currentBulletsShot = 0;
        reload = false;
    }

    //Beefy method that 
    private void Shoot()
    {
        //If the weapon is set to fire automatically, otherwise set it to fire once per trigger
        if(automaticFiring)
        {
            shooting = Input.GetMouseButton(0);
        }
        else
        {
            shooting = Input.GetMouseButtonDown(0);
        }

        //If the player shoots, the gun is able to shoot, and they are not in the process of reloading
        if (shooting && readyToShoot && !reload)
        {
            //If burst firing is enabled, count the bullets in the burst
            if(burstFiring)
            {
                currentBulletsShot++;
            }
            //If the gun is semi-automatic, trigger the singleShot bool which makes the overheat mechanic work properly
            if(!automaticFiring)
            {
                singleShot = true;
            }
            
            //Stop the gun from firing based on the fire rate
            readyToShoot = false;
            
            //Create raycast variable
            RaycastHit hit;

            //Raycast from the center of the players UI to the nearest point based on the retical
            //If hits nothing (shooting in air) Raycast to a limited distance of 50f away (bullet will delete itself by the time it gets that far)
            if (Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit, 50f))
            {
                //If the player isnt standing really close to something, shoot the bullet towards the object the raycast hit or the center 50f away
                if (Vector3.Distance(myCameraHead.position, hit.point) > 2f)
                {
                    //Make the bullet travel along the raycast line
                    firePosition.LookAt(hit.point);

                    //If object was hit and it has a bullethole associated with it, instantiate the bullethole

                    string tag = hit.collider.gameObject.tag;
                    if (StaticData.particleDictionary.ContainsKey(tag))
                    {
                        Instantiate(StaticData.particleDictionary[tag], hit.point, Quaternion.LookRotation(hit.normal));
                    }
                }

            }
            else    //If the raycast doesnt hit anything, shoot the bullet to follow the max raycast length
            {
                firePosition.LookAt(myCameraHead.position + (myCameraHead.forward * 50f));
            }

            //Muzzleflash effect
            Instantiate(muzzleFlash, firePosition.position, firePosition.rotation, firePosition);

            //Shoot bullet
            Instantiate(bullet, firePosition.position, firePosition.rotation);

            //Add heat to the weapon
            currentHeat += heatPerShot;

            //Start coroutine that handles fire rate, and overheating
            StartCoroutine(ResetReadyToShoot());
        }
    }

    //This coroutine handles the fire rate of the gun, and handles the situation where the gun overheats
    private IEnumerator ResetReadyToShoot()
    {
        //Pause the coroutine based on the fire rate. If nothing else goes wrong the gun will be allowed to shoot again
        yield return new WaitForSeconds(1/fireRate);

        //If the gun is set to burst fire, and the burst has finished, pause based on the delay and reset the bullet count
        if(burstFiring && currentBulletsShot >= bulletsPerBurst)
        {
            yield return new WaitForSeconds(burstFireDelay/1000);
            currentBulletsShot = 0;
        }

        //If the gun overheated, pause for the overheat cooldown param then reset the heat values/reset bullet count if its burst
        if(currentHeat >= maxHeat)
        {
            yield return new WaitForSeconds(overheatCooldown/1000);
            currentHeat = 0;
            currentBulletsShot = 0;
        }

        //The singleShot value is meant to make sure PassiveCooldown coroutine doesnt start up between shots of a semi-automatic weapon
        //Otherwise you would have to click really fast and have a high enough fire rate to outpace the cooldown from heating up
        singleShot = false;

        //Let the gun shoot again
        readyToShoot = true;
    }

    //This coroutine is in charge of slowly reducing heat when the player is NOT shooting
    private IEnumerator PassiveCooldown()
    {
        while(true)
        {
            //If the player is shooting, the gun overheated, is reloading, or a semi-automatic gun is between shots. Do not cool
            if(shooting || currentHeat >= maxHeat || reload || singleShot)
            {
                //This while loop blocks execution such that the next line will only run when all the values are false
                while(shooting || currentHeat >= maxHeat || reload || singleShot)
                {
                    yield return null;
                }
                //Brief delay before the cooldown begins
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                //cooldown the weapon based on the coolDownPerSecond param in increments of 1/5 (This is just for asthetics)
                float deduction = (currentHeat - (coolDownPerSecond / 5));

                //This just makes sure the currentHeat value is never lower than 0
                currentHeat = deduction < 0 ? 0 : deduction; 

                //Wait 1/5 of a second
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    //If the player reloads, wait the reload time then call the Reload() function
    private IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(manualCooldown / 1000);
        reload = false;
        Reload();
    }
}
