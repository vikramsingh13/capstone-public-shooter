using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using System;

public class Weapon : MonoBehaviour
{
    //Public Inspector Variables-----------------------------------------------------------------------------
    //Specifications for the Weapon
    [Space(4)]
    [Header("Weapon Specifications")]

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

    [Tooltip("Activates when the weapon is unavailable, either due to reloading or overheating.")]
    public bool weaponUnavailable = false;

    //Required Attributes for the Weapon

    [Space(4)]
    [Header("Required Gun Attributes to function")]

    [Tooltip("Required for raycasting and bullet accuracy")]
    [SerializeField]
    private Transform _playerEyes;

    [Space(1)]
    [Header("   Bullet Attributes")]

    [Tooltip("Set the bullet GameObject here, a prefab of any bullet object will work so long as it has a RigidBody")]
    public GameObject bullet;

    [Tooltip("Set this to the transform of a GameObject to decide where the bullets will spawn when instantiated\n\nThis also determins where the muzzle flash is drawn")]
    public Transform firePosition;

    [Tooltip("Set the muzzflash particle effect GameObject")]
    public ParticleSystem muzzleFlash;

    public TMP_Text heatText;
    public TMP_Text heatMonitorText;

    public bool hitScan = false;

    public float damage = 25f;

    [SerializeField] GameObject hitEffect;  // The gameobject to instantiate at the hit location

    public AudioSource audioSource;   // The audio source to play when firing

    public AudioClip gunShot; // Update is called once per frame

    public AudioClip overheatAlarm;

    //[Tooltip("Set a reference to the staticVariable library, to access particle effects")]
    //public GameObject BulletImpactStaticData;
    //private StaticData staticData;

    //Private Member Variables-----------------------------------------------------------------------------
    private bool shooting, readyToShoot = true, reload = false, singleShot=false;
    private int currentBulletsShot = 0;
    public float currentHeat;

    // Start is called before the first frame update
    void Start()
    {
        //audioSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        if (burstFiring)
        {
            automaticFiring = true;
        }

        StartCoroutine(HeatCooldown());

        //TODO: this will be passed to weapon when equipping.
        //for now grab the reference with Tag
        try
        {
            _playerEyes = GameObject.FindWithTag("PlayerEyes").transform;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Following Exception occurred trying to get playerEyes : {_playerEyes} in Weapon::Start : {ex.Message}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            _playerEyes = GameObject.FindWithTag("PlayerEyes").transform;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Following Exception occurred trying to get playerEyes : {_playerEyes} in Weapon::Start : {ex.Message}");
        }

        Shoot();
        GunManager();
        heatText.text = "Heat:\n" + currentHeat + " / " + maxHeat;
        
        if(weaponUnavailable)
        {
            heatMonitorText.text = "!!!";
        }
        else
        {
            heatMonitorText.text = currentHeat.ToString("0");
        }

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
        UsePrimaryFire();
    }

    private void UsePrimaryFire()
    {

        //If the player shoots, the gun is able to shoot, and they are not in the process of reloading
        if (shooting && readyToShoot && !reload)
        {
            muzzleFlash.Play();
            audioSource.PlayOneShot(gunShot);

            //AudioManager.instance.PlaySFX(audioSource, gunShot);

            //If burst firing is enabled, count the bullets in the burst
            if (burstFiring)
            {
                currentBulletsShot++;
            }
            //If the gun is semi-automatic, trigger the singleShot bool which makes the overheat mechanic work properly
            if (!automaticFiring)
            {
                singleShot = true;
            }

            //Stop the gun from firing based on the fire rate
            readyToShoot = false;

            //Create raycast variable
            RaycastHit hit;

            //Raycast from the center of the players UI to the nearest point based on the retical
            //If hits nothing (shooting in air) Raycast to a limited distance of 50f away (bullet will delete itself by the time it gets that far)
            if (Physics.Raycast(_playerEyes.position, _playerEyes.forward, out hit, 100f))
            {
                if (hitScan)
                {
                    GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(impact, .1F);  // Destroy the hit effect after a short delay
                    EnemyHealth target = hit.transform.GetComponent<EnemyHealth>();  // Get the EnemyHealth component of the hit object
                    if (target != null)
                    {
                        target.TakeDamage(damage);
                    }
                }
                else
                {
                    //If the player isnt standing really close to something, shoot the bullet towards the object the raycast hit or the center 50f away
                    /*
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
                    }*/
                }

            }
            else    //If the raycast doesnt hit anything, shoot the bullet to follow the max raycast length
            {
                if (!hitScan)
                {
                    firePosition.LookAt(_playerEyes.position + (_playerEyes.forward * 100f));
                }
            }

            //Muzzleflash effect
            //Instantiate(muzzleFlash, firePosition.position, firePosition.rotation, firePosition);

            //Shoot bullet
            if (!hitScan)
            {
                Instantiate(bullet, firePosition.position, firePosition.rotation);
            }

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
            weaponUnavailable = true;
            yield return new WaitForSeconds(overheatCooldown/1000);
            currentHeat = 0;
            currentBulletsShot = 0;
        }

        //The singleShot value is meant to make sure PassiveCooldown coroutine doesnt start up between shots of a semi-automatic weapon
        //Otherwise you would have to click really fast and have a high enough fire rate to outpace the cooldown from heating up
        singleShot = false;

        //Let the gun shoot again
        readyToShoot = true;

        //Flags the gun as being available for use.
        weaponUnavailable = false;
    }

    //This coroutine is in charge of slowly reducing heat when the player is NOT shooting
    //TODO: REFACTOR THIS TO PLAYER.CS
    private IEnumerator HeatCooldown()
    {
        float cooldownTime = 0.1f; // cooldown time in seconds
        float deductionAmount = coolDownPerSecond * cooldownTime;
        
        while (true)
        {
            if (!shooting && currentHeat < maxHeat && !reload && !singleShot)
            {
                // gradually decrease the heat value
                if (currentHeat > 0)
                {
                    currentHeat -= deductionAmount;
                    yield return new WaitForSeconds(cooldownTime);
                }
                else
                {
                    // make sure the currentHeat value is never negative
                    currentHeat = 0f;
                }
            }

            yield return null;
        }
    }

    //If the player reloads, wait the reload time then call the Reload() function
    //TODO: REFACTOR THIS COROUTINE SO IT'S REUSABLE
    private IEnumerator ReloadCoroutine()
    {
        weaponUnavailable = true;
        yield return new WaitForSeconds(manualCooldown / 1000);
        reload = false;
        weaponUnavailable = false;
        Reload();
    }
}
