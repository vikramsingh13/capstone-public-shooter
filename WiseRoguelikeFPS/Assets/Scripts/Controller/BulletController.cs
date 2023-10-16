using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

public class BulletController : MonoBehaviour
{

    public float speed;
    public float bulletLife;
    public int bulletDamage;

    public Rigidbody bulletBody;

    [SerializeField]
    private GameManager _gameManager;


    [Inject]
    public void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        BulletMovement();

        bulletLife -= Time.deltaTime;

        if (bulletLife < 0)
        {
            Destroy(gameObject);
        }
    }

    private void BulletMovement()
    {
        bulletBody.velocity = transform.forward * speed;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyHealth target = other.transform.GetComponent<EnemyHealth>();  // Get the EnemyHealth component of the hit object
            target.TakeDamage(bulletDamage);
        }
        Destroy(this);
    }
    public void LoadProjectile(string projectileName, AssetReference projectileReference, GameObject source, Transform pos, Vector3 targetPos, bool isFiredByPlayer)
    {
        if (_gameManager != null && !_gameManager.IsPaused)
        {
            AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(projectileReference);
            handle.Completed += (operation) => OnProjectileLoaded(operation, projectileName, source, pos, targetPos, isFiredByPlayer);
        }
        else
        {
            Debug.LogError($"Projectile data not found for projectile name: {projectileName}");
        }
    }

    private void OnProjectileLoaded(AsyncOperationHandle<GameObject> handle, string projectileName, GameObject source, Transform pos, Vector3 targetPos, bool isFiredByPlayer)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            //enemy object could have been destroyed by the time this is called
            //no need to check if the Transform ref exists or is it null
            if (pos != null)
            {
                Projectile projectile = Instantiate(handle.Result, pos.position, pos.rotation).GetComponent<Projectile>();
                projectile.Init(false, projectileName, source, 10, 2, targetPos, isFiredByPlayer);
                projectile.Fire();
            }
        }
    }
}
