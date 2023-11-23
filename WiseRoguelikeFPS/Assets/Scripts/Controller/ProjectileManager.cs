using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using System.Threading.Tasks;
using System;

public class ProjectileManager : Singleton<ProjectileManager>
{
    //List that holds all the activeProjectiles. Nulls will be removed at update
    private List<GameObject> activeProjectiles = new List<GameObject>();
    
    private GameManager _gameManager;
    private int _totalProjectiles = 1;

    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    void Update()
    {
        //Remove nulls from activeProjectiles list
        for (int i = activeProjectiles.Count - 1; i >= 0; i--)
        {
            if (activeProjectiles[i] == null)
            {
                activeProjectiles.RemoveAt(i);
            }
        }
    }

    public async Task LoadAndInstantiateProjectile(string projectileDataAddressable, Vector3 directionOfTravel, Quaternion quaternionRotation, Transform projectileOrigin, GameObject firedByGameObject, float projectileDamage, bool isFiredByPlayer = true, int totalProjectiles = 1)
    {
        _totalProjectiles = totalProjectiles;
        //Load the ScriptableObject that contains the address to the projectile prefab
        Addressables.LoadAssetAsync<ProjectileData>(projectileDataAddressable).Completed += (handle) =>
        {
            try
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    ProjectileData projectileData = handle.Result;
                    string projectilePrefabAddressable = projectileData.projectilePrefabAddress;

                    //Load and instantiate the projectile prefab
                    Addressables.LoadAssetAsync<GameObject>(projectilePrefabAddressable).Completed += (prefabHandle) =>
                    {
                        if (prefabHandle.Status == AsyncOperationStatus.Succeeded)
                        {
                            GameObject projectilePrefab = prefabHandle.Result;

                            Debug.Log($"totalProjectiles: {_totalProjectiles}");
                            //loop for totalProjectiles
                            for (int i = 0; i < _totalProjectiles; i++)
                            {
                                // Randomize an offset within the range [0, 1]
                                float randomY = UnityEngine.Random.Range(-.05f, .1f);
                                float randomX = UnityEngine.Random.Range(-.1f, .1f);
                                float randomZ = UnityEngine.Random.Range(-.1f, .1f);
                                if (_totalProjectiles > 1)
                                {
                                    directionOfTravel.y += randomY;
                                    directionOfTravel.x += randomX;
                                    directionOfTravel.z += randomZ;
                                }

                                //by the time the async operation is complete, the projectileOrigin may have been destroyed
                                try
                                {

                                //Instantiate the projectile prefab
                                GameObject projectile = Instantiate(projectilePrefab, projectileOrigin.position, quaternionRotation);
                                activeProjectiles.Add(projectile);

                                //Pass the direction of travel and projectile data to the projectile
                                projectile.GetComponent<Projectile>().Init(projectileData, directionOfTravel, firedByGameObject, projectileDamage, isFiredByPlayer);
                                }
                                catch (Exception e)
                                {
                                    Debug.Log($"Failed to instantiate projectile prefab: {projectilePrefabAddressable} for {e}");
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError($"Failed to load projectile data in ProjectileManager::LoadAndInstantiateProjectile for address: {projectilePrefabAddressable}");
                        }
                    };
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load projectile data in ProjectileManager::LoadAndInstantiateProjectile for address: {projectileDataAddressable}");
            }
        };
    }
}