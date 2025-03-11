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
    AssetReferenceGameObject m_EnemyObj;

    [SerializeField]
    Transform m_SetEnemyPosition1;
    [SerializeField]
    Transform m_SetEnemyPosition2;
    [SerializeField]
    GameObject m_Enemys;

    [SerializeField]
    public Transform TargetPosition;

    [SerializeField] int m_PoolSize = 100;

    List<Enemy> m_EnemyList = new List<Enemy>();

    float m_SpawnCooldown = 1f;
    float m_LastSpawnTime = 0f;
    int m_LoadedCount = 0;
    void Awake()
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
        if (Time.time - m_LastSpawnTime >= m_SpawnCooldown)
        {
            ActivateRandomEnemy();
            m_LastSpawnTime = Time.time;
        }
    }

    void InitObject()
    {
        for (int i = 0; i < m_PoolSize; i++)
        {
            m_EnemyObj.InstantiateAsync(m_Enemys.transform).Completed += obj =>
            {
                if (obj.Status == AsyncOperationStatus.Succeeded)
                {
                    GameObject enemyObj = obj.Result;
                    Enemy enemy = enemyObj.GetComponent<Enemy>();
                    enemyObj.SetActive(false);
                    m_EnemyList.Add(enemy);
                    m_LoadedCount++;
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
                spawnPosition = m_SetEnemyPosition1;
                randomEnemy.gameObject.layer = LayerMask.NameToLayer("Enemy1");
            }
            else
            {
                spawnPosition = m_SetEnemyPosition2;
                randomEnemy.gameObject.layer = LayerMask.NameToLayer("Enemy2");
            }

            randomEnemy.gameObject.SetActive(true);
            randomEnemy.StartMove(spawnPosition.position);
        }
    }

    Enemy GetRandomInactiveEnemy()
    {
        foreach (var enemy in m_EnemyList)
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
