using UnityEngine;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int enemyCount = 10000;
    [SerializeField] private Vector2 spawnAreaSize = new Vector2(500f, 500f);
    [SerializeField] private float activationRadius = 100f;
    [SerializeField] private Transform player;
    [SerializeField] private int gridCellSize = 50;

    private List<Enemy> allEnemies = new List<Enemy>();
    private Dictionary<Vector2Int, List<Enemy>> spatialGrid = new Dictionary<Vector2Int, List<Enemy>>();

    public Transform GetPlayerTransform()
    {
        return player;
    }

    void Start()
    {
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
        float x = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float z = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
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
        
        AddToGrid(enemy, newPos);
    }
}
