using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 10000;
    [SerializeField] private Transform spawnArea;
    [SerializeField] private float activationRadius = 100f;
    [SerializeField] private Transform player;
    [SerializeField] private int gridCellSize = 50;

    private List<Enemy> allEnemies = new List<Enemy>();
    private Dictionary<Vector2Int, List<Enemy>> spatialGrid = new Dictionary<Vector2Int, List<Enemy>>();

    public Transform GetPlayerTransform()
    {
        return player;
    }

    void Awake()
    {
        if (spawnArea == null)
        {
            Debug.LogError("[EnemyManager] Spawn Area 未赋值，刷怪逻辑已禁用。请在 Inspector 中分配 SpawnArea GameObject。", this);
        }
    }

    void Start()
    {
        if (spawnArea == null)
        {
            Debug.LogWarning("[EnemyManager] 由于 Spawn Area 缺失，跳过刷怪。", this);
            return;
        }

        SpawnEnemies();
    }

    void Update()
    {
        UpdateEnemyActivation();
    }

    void SpawnEnemies()
    {
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 randomPos = GetRandomGroundPosition();
            GameObject enemyObj = Instantiate(enemyPrefab, randomPos, Quaternion.identity, transform);
            Enemy enemy = enemyObj.GetComponent<Enemy>();
            
            if (enemy != null)
            {
                enemy.Initialize(this);
                enemy.Deactivate();
                allEnemies.Add(enemy);
                AddToGrid(enemy, randomPos);
            }
        }
    }

    Vector3 GetRandomGroundPosition()
    {
        if (spawnArea == null)
        {
            Debug.LogError("[EnemyManager] GetRandomGroundPosition 被调用，但 spawnArea 为空。", this);
            return Vector3.zero;
        }

        // 用 Renderer.bounds 自动计算旋转+缩放后的世界空间包围盒
        Renderer renderer = spawnArea.GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("[EnemyManager] SpawnArea 没有 Renderer 组件，无法计算刷怪范围。", this);
            return spawnArea.position;
        }

        Bounds bounds = renderer.bounds;

        // 在包围盒内随机选点（XZ 平面）
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        return new Vector3(x, 0.5f, z);
    }

    void AddToGrid(Enemy enemy, Vector3 position)
    {
        Vector2Int gridCell = GetGridCell(position);
        if (!spatialGrid.ContainsKey(gridCell))
        {
            spatialGrid[gridCell] = new List<Enemy>();
        }
        spatialGrid[gridCell].Add(enemy);
    }

    void RemoveFromGrid(Enemy enemy, Vector3 position)
    {
        Vector2Int gridCell = GetGridCell(position);
        if (spatialGrid.ContainsKey(gridCell))
        {
            spatialGrid[gridCell].Remove(enemy);
        }
    }

    Vector2Int GetGridCell(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / gridCellSize);
        int z = Mathf.FloorToInt(worldPos.z / gridCellSize);
        return new Vector2Int(x, z);
    }

    void UpdateEnemyActivation()
    {
        if (player == null) return;

        Vector3 playerPos = player.position;
        Vector2Int playerCell = GetGridCell(playerPos);
        
        int cellRadius = Mathf.CeilToInt(activationRadius / gridCellSize);

        for (int x = -cellRadius; x <= cellRadius; x++)
        {
            for (int z = -cellRadius; z <= cellRadius; z++)
            {
                Vector2Int checkCell = new Vector2Int(playerCell.x + x, playerCell.y + z);
                if (spatialGrid.ContainsKey(checkCell))
                {
                    foreach (Enemy enemy in spatialGrid[checkCell])
                    {
                        if (enemy == null) continue;

                        float distance = Vector3.Distance(playerPos, enemy.transform.position);
                        
                        if (distance <= activationRadius && !enemy.IsActive)
                        {
                            enemy.Activate();
                        }
                        else if (distance > activationRadius * 1.2f && enemy.IsActive)
                        {
                            enemy.Deactivate();
                        }
                    }
                }
            }
        }
    }

    public void RespawnEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        Vector3 oldPos = enemy.transform.position;
        RemoveFromGrid(enemy, oldPos);

        Vector3 newPos = GetRandomGroundPosition();
        enemy.transform.position = newPos;
        enemy.Deactivate();
        enemy.ResetState();

        AddToGrid(enemy, newPos);
    }
}
