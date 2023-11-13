using System;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.XInput;
using UnityEngine.ProBuilder;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class Player : DamageableEntity
{

    private PlayerMovement _playerMovement;
    private PlayerStats _stats;
    private PlayerManager _playerManager;
    private AudioManager _audioManager;
    private GameManager _gameManager;
    private DiContainer _diContainer;

    //Weapons and heat
    private GameObject _equippedWeaponGameObject;
    private List<GameObject> _listOfLoadoutWeaponsGameObjects;
    //temporary. Grab from PlayerData
    private float _maxHeat = 100f;
    private float _currentHeat;

    //public Transform testSlope;
    public const float BASE_SPRINTSPEEDMOD = 1.5f;
    public const float BASE_CROUCHSPEEDMOD = 0.75f;

    public const float BASE_HEALTH = 100f;

    private float sprintSpeedMod;
    private float crouchSpeedMod;

    //Input keys can be refactored to scriptable objects 
    //this way player modified settings can be saved. 
    private KeyCode kc_jump = KeyCode.Space;
    private KeyCode kc_sprint = KeyCode.LeftShift;
    private KeyCode kc_crouch = KeyCode.LeftControl;

    [Inject]
    public void Construct(PlayerManager playerManager, GameManager gameManager, AudioManager audioManager, DiContainer diContainer)
    {
        _diContainer = diContainer;
        Debug.Log("_player init: ");
        _playerManager = playerManager;
        Debug.Log("_player init manager: " + _playerManager);
        _gameManager = gameManager;
        _audioManager = audioManager;
        _stats = GetComponent<PlayerStats>();

        this.sprintSpeedMod = BASE_SPRINTSPEEDMOD;
        this.crouchSpeedMod = BASE_CROUCHSPEEDMOD;
        base._health = BASE_HEALTH;
    }

    //when we need to dynamically init the player
    public void Init(PlayerManager playerManager, GameManager gameManager, AudioManager audioManager, DiContainer diContainer)
    {
        _diContainer = diContainer;
        Debug.Log("_player init: ");
        _playerManager = playerManager;
        Debug.Log("_player init manager: " + _playerManager);
        _gameManager = gameManager;
        _audioManager = audioManager;

        this.sprintSpeedMod = BASE_SPRINTSPEEDMOD;
        this.crouchSpeedMod = BASE_CROUCHSPEEDMOD;
        base._health = BASE_HEALTH;
    }

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        //find the equipped weapon at the start
        //TODO: refactor to support weapon swapping
    }

    // Update is called once per frame
    void Update()
    {
        if(_playerMovement == null)
        {
            _playerMovement = GetComponent<PlayerMovement>();
        }
        //all player updates are disabled when game is paused
        if (_gameManager != null && _gameManager.IsPaused) return;

        //Fire Weapon based on mouse button clicks
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            FireWeapon(Input.GetMouseButtonDown(1));
        }

        //TODO: REFACTOR THIS for manually added weapons
        //get equipped weapon if it's missing
        if(_equippedWeaponGameObject == null)
        {
            this.LoadAndAttachWeapon("Assets/Data/Weapons/PlasmaGun.asset");

        }

        //Movement checks for PlayerMovement class
        _playerMovement.Move(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"),
            10,
            10,
            sprintSpeedMod,
            crouchSpeedMod,
            Input.GetKey(kc_sprint),
            Input.GetKey(kc_crouch),
            Input.GetKey(kc_jump)
        );
    }

    //Calls the equippedWeapon's weapon's class's Fire method
    //Logs debug and does nothing else if no equipped weapon
    //Logs debug and does nothing else if current heat is greater than max heat from player stats/data
    private async void FireWeapon(bool useSecondaryFire = false)
    {
        if (_equippedWeaponGameObject != null)
        {
            if(_currentHeat < _maxHeat)
            {
                try
                {
                    float heatGenerated = await _equippedWeaponGameObject.GetComponent<Weapon>().Fire(useSecondaryFire);

                    ModifyCurrentHeat(heatGenerated);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Encountered the following exception trying to use Weapon::Fire from _equippedWeaponGameObject : {_equippedWeaponGameObject}. Exception message: " + ex.Message);
                }
            }
            else
            {
                Debug.Log($"Player current heat {_currentHeat} has reached max heat {_maxHeat} threshold. Cannot use any weapons!");
            }
        }
        else
        {
            Debug.Log("No equipped weapon in Player::FireWeapon");
        }
    }

    //takes heatChange as positive or negative floats
    private void ModifyCurrentHeat(float heatChange)
    {
        _currentHeat = _currentHeat + heatChange;

        if(_currentHeat < 0)
        {
            _currentHeat = 0;
        }
    }

    //TEMP CODE FOR DI INSTANTIATE WEAPONS MANUALLY ADDED TO THE SCENE
    public void LoadAndAttachWeapon(string weaponDataAddressableKey)
    {
        Addressables.LoadAssetAsync<WeaponData>(weaponDataAddressableKey).Completed += weaponDataHandle =>
        {
            if (weaponDataHandle.Status == AsyncOperationStatus.Succeeded)
            {
                WeaponData weaponData = weaponDataHandle.Result;
                Addressables.InstantiateAsync(weaponData.weaponPrefabAddress).Completed += weaponPrefabHandle =>
                {
                    if (weaponPrefabHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject weaponPrefab = weaponPrefabHandle.Result;
                        Transform gunParentTransform = transform.Find("GunParent");
                        if (gunParentTransform != null)
                        {
                            weaponPrefab.transform.SetParent(gunParentTransform, false);
                            _diContainer.InjectGameObjectForComponent<Weapon>(weaponPrefab);

                            //set the newly loaded weapon as equipped
                            _equippedWeaponGameObject = weaponPrefab;
                        }
                        else
                        {
                            Debug.LogError("Gun Parent not found");
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to instantiate weapon prefab");
                    }
                };
            }
            else
            {
                Debug.LogError("Failed to load WeaponData");
            }
        };
    }

    //This coroutine is in charge of slowly reducing heat when the player is NOT shooting
    /*private IEnumerator HeatCooldown()
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
    }*/
}
