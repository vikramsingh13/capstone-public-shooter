using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ProjectileManager : Singleton<ProjectileManager>
{
    //List that holds all the activeProjectiles. Nulls will be removed at update
    private List<GameObject> activeProjectiles = new List<GameObject>();

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

    public void LoadAndInstantiateProjectile(string addressableKey, Vector3 position, Quaternion rotation)
    {
        Addressables.LoadAssetAsync<GameObject>(addressableKey).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject projectilePrefab = handle.Result;

                GameObject projectile = Instantiate(projectilePrefab, position, rotation);
                activeProjectiles.Add(projectile);

                // Assuming the projectile has a component script with an Init method
                projectile.GetComponent<Projectile>().Init();
            }
            else
            {
                Debug.LogError($"Failed to load projectile: {handle.Status}");
            }
        };
    }
}