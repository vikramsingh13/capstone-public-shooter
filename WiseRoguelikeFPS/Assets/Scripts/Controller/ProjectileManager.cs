using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using System.Threading.Tasks;

public class ProjectileManager : Singleton<ProjectileManager>
{
    //List that holds all the activeProjectiles. Nulls will be removed at update
    private List<GameObject> activeProjectiles = new List<GameObject>();
    
    private GameManager _gameManager;

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

    public async Task LoadAndInstantiateProjectile(string projectileDataAddressable, Vector3 directionOfTravel, Quaternion quaternionRotation, Transform projectileOrigin, GameObject firedByGameObject, float projectileDamage, bool isFiredByPlayer = true)
    {
        //Load the ScriptableObject that contains the address to the projectile prefab
        Addressables.LoadAssetAsync<ProjectileData>(projectileDataAddressable).Completed += (handle) =>
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

                        GameObject projectile = Instantiate(projectilePrefab, projectileOrigin.position, quaternionRotation);
                        activeProjectiles.Add(projectile);

                        //Pass the direction of travel and projectile data to the projectile
                        projectile.GetComponent<Projectile>().Init(projectileData, directionOfTravel, firedByGameObject, projectileDamage, isFiredByPlayer);
                    }
                    else
                    {
                        Debug.LogError($"Failed to load projectile data in ProjectileManager::LoadAndInstantiateProjectile for address: {projectilePrefabAddressable}");
                    }
                };
            }
            else
            {
                Debug.LogError($"Failed to load projectile data in ProjectileManager::LoadAndInstantiateProjectile for address: {projectileDataAddressable}");
            }
        };
    }
}