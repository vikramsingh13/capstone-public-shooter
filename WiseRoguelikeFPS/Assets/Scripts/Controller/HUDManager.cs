using UnityEngine;
using Zenject;
//async operations
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

public class HUDManager : Singleton<HUDManager>
{
    private GameManager _gameManager;
    private GameObject _HUDPanel;
    private GameObject _weaponLoadoutParent;
    private List<GameObject> _weaponLoadoutSlots = new List<GameObject>();
    private GameObject _activeWeaponSlot;
    // the list of weapons we get from the player
    private List<GameObject> _weaponLoadoutList = new List<GameObject>();
    private bool _isWeaponLoadoutSet = false;
    private float _currentPlayerHealth = 0f;
    private float _newPlayerHealth = 100f;
    private GameObject _playerHealthBarUIParent;



    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    void Start()
    {
        AsyncLoadHUD();
        GetWeaponLoadoutParentAndSlots();
        GetPlayerHealthBarUIParent();

        //TODO: Refactor to this:
        //_gameManager.OnGameStateChanged.AddListener(HandleGameStateChanged);
        //currently call scenemanagement to get the current scene name and check if it is the Menu Scene
        //if menu then set the HUD to inactive
    }

    void Update()
    {
        if (_HUDPanel == null)
        {
            Debug.Log("HUDPanel is null in update");
            AsyncLoadHUD();
        }

        if(_weaponLoadoutParent == null)
        {
            Debug.Log("weapon loadout parent is null in update");
            GetWeaponLoadoutParentAndSlots();
        }

        if(_playerHealthBarUIParent == null)
        {
            GetPlayerHealthBarUIParent();
        }

        if(_weaponLoadoutList.Count > 0 && _weaponLoadoutSlots.Count > 0 && !_isWeaponLoadoutSet)
        {
            SetWeaponLoadout();
        }

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            _HUDPanel.SetActive(false);
        }

        if (SceneManager.GetActiveScene().name != "Menu")
        {
            _HUDPanel.SetActive(true);
        }

        if(_currentPlayerHealth != _newPlayerHealth)
        {
            UpdatePlayerHealthBar();
        }

    }

    private void GetPlayerHealthBarUIParent()
    {
        //Find gameobject named WeaponLoadout as a nested child in HUDPanel
        _playerHealthBarUIParent = _HUDPanel.transform.Find("HudPanel")?.transform.Find("GameUI")?.transform.Find("PlayerHealthBarUIParent").gameObject;
    }

    //public accessor method for player to update the playerHealth in HUD
    //TODO: refactor to use events
    //TODO: refactor to take the final hp% for the slider value. All health logic should be done by player.
    public void SetPlayerHealth(float playerHealth)
    {
        _newPlayerHealth = playerHealth;
    }

    private void UpdatePlayerHealthBar()
    {
        //player health should not be displayed lower than 0 or greater than 100
        _currentPlayerHealth = _newPlayerHealth >= 0 ? _newPlayerHealth : 0;
        _currentPlayerHealth = _newPlayerHealth <= 100 ? _newPlayerHealth : 100;
        if(_playerHealthBarUIParent != null)
        {
            try
            {
                Debug.Log($"Updating player health bar in HUDManager::UpdatePlayerHealthBar with _currentPlayerHealth: {_currentPlayerHealth} and _newPlayerHealth: {_newPlayerHealth}.");
                //division by hard coded 100f since the slider is set to 0-1
                _playerHealthBarUIParent.GetComponentInChildren<Slider>().value = _currentPlayerHealth / 100f;
            }
            catch (Exception ex)
            {
                Debug.Log($"Exception occurred trying to update player health bar in HUDManager::UpdatePlayerHealthBar: {ex.Message}");
            }
        }
    }

    private void GetWeaponLoadoutParentAndSlots()
    {
        //Find gameobject named WeaponLoadout as a nested child in HUDPanel
        //TODO: fix this prefab
        _weaponLoadoutParent = _HUDPanel.transform.Find("HudPanel")?.transform.Find("GameUI")?.transform.Find("WeaponLoadoutParent").gameObject;
        //Iterate through the weaponLoadoutGameObject and find it's children with "WeaponLoadoutSlot" name and set it to a variable
        if(_weaponLoadoutParent != null )
        {
            for (int i = 0; i < _weaponLoadoutParent.transform.childCount; i++)
            {
                _weaponLoadoutSlots.Add(_weaponLoadoutParent.transform.GetChild(i).gameObject);
            }
            Debug.Log("Loaded weapon loadout slots in HUDManager::GetWeaponLoadoutParentAndSlots");
        }
    }

    public void HandleSetWeaponLoadoutEvent(List<GameObject> weaponLoadoutList)
    {
        if(weaponLoadoutList.Count > 0)
        {
            _weaponLoadoutList = weaponLoadoutList;
            Debug.Log("++++ weapon loadout list came through from player");
        }
    }

    public void SetWeaponLoadout()
    {
        Debug.Log($"HandleSetWeaponLoadoutEvent called in HUDManager with weaponLoadoutList.Count: {_weaponLoadoutList.Count} and _weaponLoadoutSlots.Count: {_weaponLoadoutSlots.Count}.");

        int _minCount = Math.Min(_weaponLoadoutList.Count, _weaponLoadoutSlots.Count);
        //iterate over weaponLoadoutList and addressable async load the weaponData's weaponIconAddress and set the sprite to the weaponLoadoutSlots[i] image component
        for (int i = 0; i < _minCount; i++)
        {
            //need to capture the current index value
            //when LoadAssetAsync is completed the i value is already at the end of the loop
            int currentIndex = i;
            string weaponIconAddress = _weaponLoadoutList[currentIndex].GetComponent<Weapon>().GetWeaponData.weaponIconAddress;
            if (weaponIconAddress != null)
            {
                Addressables.LoadAssetAsync<Texture2D>(weaponIconAddress).Completed += (spriteHandle) =>
                {
                    if (spriteHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        //add the weapon sprite to the loadout slot image component
                        Texture2D weaponIcon = spriteHandle.Result;
                        Sprite weaponSprite = Sprite.Create(weaponIcon, new Rect(0, 0, weaponIcon.width, weaponIcon.height), new Vector2(0.5f, 0.5f));
                        _weaponLoadoutSlots[currentIndex].transform.Find("Icon").GetComponent<Image>().sprite = weaponSprite;
                    }
                    else
                    {
                        Debug.LogError($"Failed to load weapon icon in HUDManager::HandleSetWeaponLoadoutEvent for address: {weaponIconAddress}");
                    }
                };
                _isWeaponLoadoutSet = true;
            }
            else
            {
                _isWeaponLoadoutSet = false;
                Debug.LogError($"Failed to load weapon icon in HUDManager::HandleSetWeaponLoadoutEvent for address: {_weaponLoadoutList[currentIndex].GetComponent<Weapon>().GetWeaponData.weaponIconAddress}");
            }
        }
    }

    public void HandleActiveWeaponSwapEvent(int index)
    {
        if(_weaponLoadoutSlots.Count > index)
        {
            _activeWeaponSlot = _weaponLoadoutSlots[index];
        }
    }

    //look for the gameObject named 'HUD' in the current active scene and set it to the _HUDPanel or async load the HUD gameObject from address Assets/Prefabs/UI/HUD/HUD.prefab and set it to the _HUDPanel
    private void AsyncLoadHUD()
    {
        //look for the gameObject named 'HUD' in the current active scene and set it to the _HUDPanel
        if (GameObject.Find("HUD"))
        {
            _HUDPanel = GameObject.Find("HUD");
            Debug.Log("Found HUD");
            return;
        }
        Debug.Log("Can't find HUD. Trying to instantiate the prefab now.");

        //if no HUD found async load the HUD gameObject from address Assets/Prefabs/UI/HUD/HUD.prefab and set it to the _HUDPanel
        Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/UI/HUD/HUD.prefab").Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _HUDPanel = handle.Result;
                return;
            }
            else
            {
                Debug.LogError("Failed to load HUD prefab in HUDManager::AsyncLoadHUD");
            }
        };
    }
}