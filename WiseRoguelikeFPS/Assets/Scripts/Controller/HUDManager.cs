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
    private List<GameObject> _weaponLoadoutSlots;
    private GameObject _activeWeaponSlot;

    public delegate void WeaponSwapEvent(int index);
    public static event WeaponSwapEvent OnWeaponSwapEvent;
    //For now invoked when the weapon loadout is set/changed
    public delegate void SetWeaponLoadoutEvent(List<GameObject> weaponLoadoutList);
    public static event SetWeaponLoadoutEvent OnSetWeaponLoadoutEvent;

    

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    void Start()
    {
        Player.OnWeaponSwapEvent += HandleActiveWeaponSwapEvent;
        Player.OnSetWeaponLoadoutEvent += HandleSetWeaponLoadoutEvent;
        GetWeaponLoadoutParentAndSlots();
        AsyncLoadHUD();

        //TODO: Refactor to this:
        //_gameManager.OnGameStateChanged.AddListener(HandleGameStateChanged);
        //currently call scenemanagement to get the current scene name and check if it is the Menu Scene
        //if menu then set the HUD to inactive
        

    }

    private void GetWeaponLoadoutParentAndSlots()
    {
        //Find gameobject named WeaponLoadout as a nested child in HUDPanel
        _weaponLoadoutParent = _HUDPanel.transform.Find("WeaponLoadout").gameObject;
        //Iterate through the weaponLoadoutGameObject and find it's children with "WeaponLoadoutSlot" name and set it to a variable
        for (int i = 0; i < _weaponLoadoutParent.transform.childCount; i++)
        {
            _weaponLoadoutSlots.Add(_weaponLoadoutParent.transform.GetChild(i).gameObject);
        }
    }

    public void HandleSetWeaponLoadoutEvent(List<GameObject> weaponLoadoutList)
    {
        Debug.Log($"HandleSetWeaponLoadoutEvent called in HUDManager with weaponLoadoutList.Count: {weaponLoadoutList.Count} and _weaponLoadoutSlots.Count: {_weaponLoadoutSlots.Count}.");

        int _minCount = Math.Min(weaponLoadoutList.Count, _weaponLoadoutSlots.Count);
        //iterate over weaponLoadoutList and addressable async load the weaponData's weaponIconAddress and set the sprite to the weaponLoadoutSlots[i] image component
        for (int i = 0; i < _minCount; i++)
        {
            //need to capture the current index value
            //when LoadAssetAsync is completed the i value is already at the end of the loop
            int currentIndex = i;
            string weaponIconAddress = weaponLoadoutList[currentIndex].GetComponent<Weapon>().GetWeaponData.weaponIconAddress;
            if (weaponIconAddress != null)
            {
                Addressables.LoadAssetAsync<Sprite>(weaponIconAddress).Completed += (spriteHandle) =>
                {
                    if (spriteHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        //add the weapon sprite to the loadout slot image component
                        Sprite weaponIcon = spriteHandle.Result;
                        _weaponLoadoutSlots[currentIndex].transform.Find("Icon").GetComponent<Image>().sprite = weaponIcon;
                    }
                    else
                    {
                        Debug.LogError($"Failed to load weapon icon in HUDManager::HandleSetWeaponLoadoutEvent for address: {weaponIconAddress}");
                    }
                };
            }
            else
            {
                Debug.LogError($"Failed to load weapon icon in HUDManager::HandleSetWeaponLoadoutEvent for address: {weaponLoadoutList[currentIndex].GetComponent<Weapon>().GetWeaponData.weaponIconAddress}");
            }
        }
    }

    public void HandleActiveWeaponSwapEvent(int index)
    {
        _activeWeaponSlot = _weaponLoadoutSlots[index];
        Debug.Log($"index of active weapon: {index}. weaponLoadoutSlots is : {_weaponLoadoutSlots}");
        //TODO: make this weapon have color and the others muted grey or transparent
        Debug.Log($"HandleActiveWeaponSwap called in HUDManager with index: {index}. This ui feature is currently not implemented.");
    }

    void Update()
    {
        if (_HUDPanel == null)
        {
            AsyncLoadHUD();
        }

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            _HUDPanel.SetActive(false);
        }

        if(SceneManager.GetActiveScene().name != "Menu")
        {
            _HUDPanel.SetActive(true);
        }

    }

    //look for the gameObject named 'HUD' in the current active scene and set it to the _HUDPanel or async load the HUD gameObject from address Assets/Prefabs/UI/HUD/HUD.prefab and set it to the _HUDPanel
    private void AsyncLoadHUD()
    {
        //look for the gameObject named 'HUD' in the current active scene and set it to the _HUDPanel
        if (GameObject.Find("HUD"))
        {
            _HUDPanel = GameObject.Find("HUD");
            return;
        }

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