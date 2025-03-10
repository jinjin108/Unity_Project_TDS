using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class ObjectManager : MonoBehaviour
{
    public static ObjectManager Instance { get; private set; }
    [SerializeField]
    AssetReferenceGameObject EnemyObj;

    [SerializeField]
    Transform SetEnemyPosition1;
    [SerializeField]
    Transform SetEnemyPosition2;

    [SerializeField]
    public Transform TargetPosition;

    [SerializeField] int PoolSize = 10;

    List<Enemy> EnemyList = new List<Enemy>();

    float spawnCooldown = 3f;
    float lastSpawnTime = 0f;
    int loadedCount = 0;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    void Start()
    {
        InitObject();
    }

    void Update()
    {
        if (Time.time - lastSpawnTime >= spawnCooldown)
        {
            ActivateRandomEnemy();
            lastSpawnTime = Time.time;
        }
    }

    void InitObject()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            EnemyObj.InstantiateAsync(SetEnemyPosition1).Completed += obj =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject enemyObj = obj.Result;
                    Enemy enemy = enemyObj.GetComponent<Enemy>();
                    enemyObj.SetActive(false);
                    EnemyList.Add(enemy);
                    loadedCount++;
                }
                else
                {
                    Debug.LogError("Enemy 로딩 실패!");
                }
            };
        }
    }

    void ActivateRandomEnemy()
    {
        Enemy randomEnemy = GetRandomInactiveEnemy();

        if (randomEnemy != null)
        {
            Transform spawnPosition;
            if (Random.Range(0, 2) == 0)
            {
                spawnPosition = SetEnemyPosition1;
                randomEnemy.gameObject.layer = LayerMask.NameToLayer("Enemy1");
            }
            else
            {
                spawnPosition = SetEnemyPosition2;
                randomEnemy.gameObject.layer = LayerMask.NameToLayer("Enemy2");
            }

            randomEnemy.gameObject.SetActive(true);
            randomEnemy.StartMove(spawnPosition.position);
        }
    }

    Enemy GetRandomInactiveEnemy()
    {
        foreach (var enemy in EnemyList)
        {
            if (!enemy.gameObject.activeInHierarchy)
            {
                return enemy;
            }
        }
        return null;
    }
    public void ReturnToPool(Enemy enemy)
    {
        enemy.StopAllCoroutines();
        enemy.gameObject.SetActive(false);
    }
}
