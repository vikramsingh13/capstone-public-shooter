using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Zenject;
using System;
using System.Threading.Tasks;

public class Weapon : MonoBehaviour
{
    private Transform _mainCameraTransform;

    private bool _isReloading = false;
    private bool _canFire = true;
    private GameObject _projectileOrigin;

    [Header("Only need to be attached manually \n when manually adding weapons to maps or prefabs.")]
    [SerializeField]
    private WeaponData _weaponData;
    private ProjectileManager _projectileManager;

    [Inject]
    public void Construct(ProjectileManager projectileManager)
    {
        _projectileManager = projectileManager;
    }

    //Initializes the gun; loads the data from SO
    public void Init(WeaponData weaponData)
    {
        //weapon data will be provided by drop manager 
        //TODO: make sure weapons can be manually added to map by attaching weaponData
        _weaponData = weaponData;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetMainCameraTransform();
        GetProjectileOrigin();
    }

    // Update is called once per frame
    void Update()
    {
        if(_mainCameraTransform == null)
        {
            SetMainCameraTransform();
        }

        if(_weaponData == null)
        {
            try
            {
                _weaponData = gameObject.GetComponent<WeaponData>();
            }
            catch (Exception ex)
            {
                Debug.LogError("No weapon data found in Weapon::Update.");
            }
        }

        if(_projectileOrigin == null)
        {
            GetProjectileOrigin();
        }
    }

    //sets the main camera object transform
    private void SetMainCameraTransform()
    {
        try
        {
            _mainCameraTransform = Camera.main.gameObject.transform;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Following Exception occurred trying to get main Camera : {_mainCameraTransform} in Weapon::SetMainCameraTransform : {ex.Message}");
        }
    }

    //TODO: refactor to support multi barrel fire
    //Gets the projectile origin game object to fire from
    private void GetProjectileOrigin()
    {
        try
        {
            _projectileOrigin = transform.Find("ProjectileOrigin1").gameObject;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Following Exception occurred trying to get prefab attached projectile origin : {_projectileOrigin} in Weapon::GetProjectileOrigin : {ex.Message}");
        }
    }

    public async Task<float> Fire(bool useSecondaryFire = false)
    {
        if( _weaponData != null)
        {
            if (!_isReloading && _canFire)
            {
                PlayMuzzleFlash();
                PlayAudio();

                //Handles hitscan logic for either fire mode
                if (!useSecondaryFire ? _weaponData.isPrimaryFireHitscan : _weaponData.isSecondaryFireHitscan)
                {
                    //Create raycast variable
                    RaycastHit hit;

                    //Raycast from the center of the players UI to the nearest point based on the retical
                    //If hits nothing (shooting in air) Raycast to a limited distance of 50f away (bullet will delete itself by the time it gets that far)
                    if (Physics.Raycast(_mainCameraTransform.position, _mainCameraTransform.forward, out hit, 100f))
                    {

                        /*
                        //TODO: Fix hiteffect. Add it to weaponData
                        GameObject impact = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                        Destroy(impact, .1F);  // Destroy the hit effect after a short delay

                        //TODO: Delegate combat calc to combat manager if the hit object is Damageable
                        */
                    }
                    else
                    {
                        /*TODO implement the logic when raycast hits nothing
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
                else
                {
                    //TODO: projectile logic
                    //Instantiate a projectile that travels towards the crosshair position when fired
                    // Vector3 directionOfTravel = _mainCameraTransform.position + _mainCameraTransform.forward;
                    // Instantiate(bulletPrefab, maxRangePoint, Quaternion.identity);
                    this.GenerateProjectile();

                }

                return !useSecondaryFire ? _weaponData.heatPerPrimaryFire : _weaponData.heatPerSecondaryFire;
            }

        }
        else
        {
            Debug.LogError($"No Weapon Data found in Weapon::Fire : {_weaponData}.");
        }

        return 0;
    }

    //Takes a boolean useSecondaryFire.
    //Calls ProjectileManager to load and int a projectile
    //based on the projectileData in weaponData file
    //at the location of the projectileOrigin on the weapon prefab.
    //Direction of travel of the projectile is set towards the main
    //camera center point at the time of firing.
    private async void GenerateProjectile(bool useSecondaryFire = false)
    {
        try
        {
            if (_projectileManager != null)
            {
                if(_projectileOrigin != null)
                {
                    Vector3 directionOfTravel = (_mainCameraTransform.forward).normalized;
                    string projectileData = !useSecondaryFire ? _weaponData.primaryProjectileAddress : _weaponData.secondaryProjectileAddress;
                    await _projectileManager.LoadAndInstantiateProjectile(projectileData, directionOfTravel, Quaternion.identity, _projectileOrigin.transform);
                }
                else
                {
                    Debug.LogError($"No Projectile Origin gameObject found in Weapon::GenerateProjectile for weapon prefab: {_weaponData.weaponPrefabAddress}.");
                }
            }
            else
            {
                Debug.LogError($"No Projectile Manager reference found in Weapon::GenerateProjectile for weapon: {_weaponData.weaponName}.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void PlayMuzzleFlash(bool useSecondaryFire = false)
    {
        if (!useSecondaryFire)
        {
            //TODO: muzzle flash logic here
            //better to use ternary to get primary/secondary flashes
        }
    }

    private void PlayAudio(AudioManager audioManager = null, bool useSecondaryFire = false)
    {
        if(audioManager != null)
        {
            //TODO: audio logic here
        }
    }

    public void Reload()
    {


    }

    //TODO: REFACTOR COROUTINEs to central Ticker Class SO IT'S REUSABLE and pausable
    private IEnumerator StartCoroutine(float waitInRealTimeSeconds)
    {
        yield return new WaitForSecondsRealtime(waitInRealTimeSeconds);
    }
}
