using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem.XInput;
using UnityEngine.ProBuilder;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using static HUDManager;

public class Player : DamageableEntity
{
    private PlayerMovement _playerMovement;
    private PlayerStats _stats;
    private PlayerManager _playerManager;
    private AudioManager _audioManager;
    private GameManager _gameManager;
    private DiContainer _diContainer;
    private GameObject _weaponParent;
    private HUDManager _hudManager;
    private Level1Manager _level1Manager;

    //Weapons and heat
    private GameObject _activeWeaponGameObject;
    //init the empty list of loadout weapons
    private List<GameObject> _listOfLoadoutWeaponsGameObjects = new List<GameObject>();
    //currently the game supports max 3 weapons in the loadout
    private const int _MaxWeaponsInLoadout = 3;
    //temporary. Grab from PlayerData
    private float _maxHeat = 100f;
    private float _currentHeat;

    //public Transform testSlope;
    public const float BASE_SPRINTSPEEDMOD = 1.5f;
    public const float BASE_CROUCHSPEEDMOD = 0.75f;

    public float _currentPlayerHealth = 100f;
    private float _maxPlayerHealth = 100f;

    private float sprintSpeedMod;
    private float crouchSpeedMod;

    //list of hardcoded weaponData addressable keys
    private List<string> _listOfWeaponDataAddressableKeys = new List<string>()
    {
        "Assets/Data/Weapons/LaserRifle.asset",
        "Assets/Data/Weapons/PlasmaGun.asset",
        "Assets/Data/Weapons/GrenadeLauncher.asset",
    };
    //used to track the asynchronous loading of the loadout weapons
    private Task _asyncSetWeaponLoadoutTask;

    //Input keys can be refactored to scriptable objects 
    //this way player modified settings can be saved. 
    private KeyCode kc_jump = KeyCode.Space;
    private KeyCode kc_sprint = KeyCode.LeftShift;
    private KeyCode kc_crouch = KeyCode.LeftControl;
    //key 1 2 3 for weapons
    private KeyCode kc_weapon1 = KeyCode.Alpha1;
    private KeyCode kc_weapon2 = KeyCode.Alpha2;
    private KeyCode kc_weapon3 = KeyCode.Alpha3;
    //cheatcodes
    private KeyCode kc_godMode = KeyCode.G;
    private KeyCode kc_bossBattle = KeyCode.B;
    private KeyCode kc_highJumper = KeyCode.H;
    private KeyCode kc_justInTime = KeyCode.J;


    [Inject]
    public void Construct(PlayerManager playerManager, GameManager gameManager, AudioManager audioManager, DiContainer diContainer, HUDManager hudManager, Level1Manager level1Manager)
    {
        _diContainer = diContainer;
        _playerManager = playerManager;
        _gameManager = gameManager;
        _audioManager = audioManager;
        _hudManager = hudManager;
        _level1Manager = level1Manager;

        _stats = GetComponent<PlayerStats>();

        this.sprintSpeedMod = BASE_SPRINTSPEEDMOD;
        this.crouchSpeedMod = BASE_CROUCHSPEEDMOD;
        base._currentHealth = _maxPlayerHealth;
        base._name = "Player";
    }

    void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _weaponParent = transform.Find("WeaponParent").gameObject;

        //call async SetWeaponLoadout to load and attach the weapons to the player and check task status in update to set the weapon loadout UI and active weapon
        _asyncSetWeaponLoadoutTask = SetWeaponLoadoutAsync();

        //set the initial player health
        //TODO: only the hp % should be sent to the HUD. Health bar logic will be done by Player. 
        _hudManager.SetPlayerHealth(_maxPlayerHealth);

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

        //when loadout weapons are async loaded and attached to the player, set the weapon loadout UI and active weapon UI
        //TODO: refactor to fix the async call in update
        if(_asyncSetWeaponLoadoutTask != null && _asyncSetWeaponLoadoutTask.IsCompleted && _listOfLoadoutWeaponsGameObjects != null && _listOfLoadoutWeaponsGameObjects.Count == 3)
        {
            //if atleast 1 weapon is found in the loadout, set the first weapon as equipped
            if (_listOfLoadoutWeaponsGameObjects.Count > 0)
            {
                _hudManager.HandleSetWeaponLoadoutEvent(_listOfLoadoutWeaponsGameObjects);
                //SwapEquippedWeapon also calls hudManager.HandleActiveWeaponSwapEvent
                SwapEquippedWeapon(0);
            }
            else
            {
                Debug.Log($"No weapons found in Player::SetWeaponLoadout after _asyncSetWeaponLoadoutTask.IsCompleted");
            }
            //remove the async task after completion
            _asyncSetWeaponLoadoutTask = null;

        }


        //all player updates are disabled when game is paused
        if (_gameManager != null && _gameManager.IsPaused) return;

        //Fire Weapon based on mouse button clicks
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            FireWeapon(Input.GetMouseButtonDown(1));
        }

        //Movement checks for PlayerMovement class
        _playerMovement.Move(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical"),
            _stats.MovementSpeed.GetCurrentValue(),
            _stats.JumpHeight.GetCurrentValue(),
            sprintSpeedMod,
            crouchSpeedMod,
            Input.GetKey(kc_sprint),
            Input.GetKey(kc_crouch),
            Input.GetKey(kc_jump)
        );

        //TODO: refactor all cheatcodes to update game state in GameManager instead of direct calls
        //Cheatcodes
        if (Input.GetKey(kc_bossBattle))
        {
            //TODO: refactor to use events -- helps avoid dealing with protection levels and direct calls
            _level1Manager.FinishLevelObjective();
            _level1Manager.FinishLevelObjective();
            _level1Manager.FinishLevelObjective();
        }

        //Weapon swap checks
        if (Input.GetKeyDown(kc_weapon1))
        {
            SwapEquippedWeapon(0);
        }
        else if (Input.GetKeyDown(kc_weapon2))
        {
            SwapEquippedWeapon(1);
        }
        else if (Input.GetKeyDown(kc_weapon3))
        {
            SwapEquippedWeapon(2);
        }
    }

    //Calls the equippedWeapon's weapon's class's Fire method
    //Logs debug and does nothing else if no equipped weapon
    //Logs debug and does nothing else if current heat is greater than max heat from player stats/data
    private async void FireWeapon(bool useSecondaryFire = false)
    {
        if (_activeWeaponGameObject != null)
        {
            if(_currentHeat < _maxHeat)
            {
                try
                {
                    float heatGenerated = await _activeWeaponGameObject.GetComponent<Weapon>().Fire(useSecondaryFire);

                    ModifyCurrentHeat(heatGenerated);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Encountered the following exception trying to use Weapon::Fire from _equippedWeaponGameObject : {_activeWeaponGameObject}. Exception message: " + ex.Message);
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
                        if (_weaponParent != null)
                        {
                            weaponPrefab.transform.SetParent(_weaponParent.transform, false);
                            _diContainer.InjectGameObjectForComponent<Weapon>(weaponPrefab);

                            //set the newly loaded weapon as equipped
                            _activeWeaponGameObject = weaponPrefab;
                        }
                        else
                        {
                            Debug.LogError("Weapon Parent not found");
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to instantiate weapon prefab");
                    }
                };

                Debug.Log($"WeaponData loaded successfully: {weaponData.name}");
            }
            else
            {
                Debug.LogError("Failed to load WeaponData");
            }
        };
    }

    //Parses through the gun parent and finds the first 3 weapon gameObject and adds them to _listLoadoutWeaponsGameObjects
    private async Task SetWeaponLoadoutAsync()
    {
        if (_weaponParent != null)
        {
            //iterate through the weapon parent and add the first 3 weapons to the loadout list and init and add the data and prefabs
            //also invoke HUDManager events to set the weapon loadout UI and active weapon UI
            for (int i = 0; i < _MaxWeaponsInLoadout && i < _listOfWeaponDataAddressableKeys.Count; i++)
            {
                //async load in DiContainer the addressableWeaponData from _listOfWeaponDataAddressableKeys
                Addressables.LoadAssetAsync<WeaponData>(_listOfWeaponDataAddressableKeys[i]).Completed += weaponDataHandle =>
                {
                    if (weaponDataHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        WeaponData weaponData = weaponDataHandle.Result;
                        Addressables.InstantiateAsync(weaponData.weaponPrefabAddress).Completed += weaponPrefabHandle =>
                        {
                            if (weaponPrefabHandle.Status == AsyncOperationStatus.Succeeded)
                            {
                                GameObject weaponPrefab = weaponPrefabHandle.Result;
                                if (_weaponParent != null)
                                {
                                    weaponPrefab.transform.SetParent(_weaponParent.transform, false);
                                    _diContainer.InjectGameObjectForComponent<Weapon>(weaponPrefab);

                                    //add the newly loaded weapon to the loadout list
                                    //to _listOfLoadoutWeaponsGameObjects[i] if it's not null
                                    // or to the end of the list if it is null
                                    if (i < _listOfLoadoutWeaponsGameObjects.Count)
                                    {
                                        _listOfLoadoutWeaponsGameObjects[i] = weaponPrefab;
                                    }
                                    else
                                    {
                                        _listOfLoadoutWeaponsGameObjects.Add(weaponPrefab);
                                    }
                                    //Init the weapon with the weaponData
                                    weaponPrefab.GetComponent<Weapon>().Init(weaponData);
                                }
                                else
                                {
                                    Debug.LogError("Weapon Parent not found");
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
            Debug.Log($"SetWeaponLoadoutAsync completed");
        }
        else
        {
            Debug.Log($"No weapon parent found in Player::SetWeaponLoadout");
        }
    }

    //Swaps the equipped weapon to the weapon at the index in the _listOfLoadoutWeaponsGameObjects
    //Does nothing if the index is out of bounds
    private void SwapEquippedWeapon(int index)
    {
        if (index >= 0 && index < _listOfLoadoutWeaponsGameObjects.Count)
        {
            if(_activeWeaponGameObject != _listOfLoadoutWeaponsGameObjects[index])
            {
                //iterate through the list of loadout weapons and set everything to false except for the index
                for (int i = 0; i < _listOfLoadoutWeaponsGameObjects.Count; i++)
                {
                    if (i == index)
                    {
                        _listOfLoadoutWeaponsGameObjects[i].SetActive(true);
                        _activeWeaponGameObject = _listOfLoadoutWeaponsGameObjects[index];
                        _hudManager.HandleActiveWeaponSwapEvent(index);
                        Debug.Log($"Swapped equipped weapon to index: {index}");
                    }
                    else
                    {
                        _listOfLoadoutWeaponsGameObjects[i].SetActive(false);
                    }
                }
            }
        }
        else
        {
            Debug.Log($"Index {index} is out of bounds in Player::SwapEquippedWeapon");
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        _hudManager.SetPlayerHealth(_currentHealth);
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
