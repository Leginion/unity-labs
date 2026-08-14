using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private GameEventSO onEnemyDeathEvent;
    [SerializeField] private float chaseSpeed = 2f;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;

    private int currentHealth;
    private EnemyManager manager;
    private bool isActive;
    private Transform player;
    private Renderer enemyRenderer;
    private Color originalColor;
    private bool isFlashing;

    public bool IsActive => isActive;

    void Awake()
    {
        currentHealth = maxHealth;
        enemyRenderer = GetComponent<Renderer>();
        if (enemyRenderer != null)
        {
            // 用 sharedMaterial 读色，避免 Awake 阶段就为每个敌人克隆一份材质实例
            originalColor = enemyRenderer.sharedMaterial.color;
        }
    }

    void Update()
    {
        if (isActive && player != null)
        {
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;
        transform.position += direction * chaseSpeed * Time.deltaTime;
    }

    public void Initialize(EnemyManager enemyManager)
    {
        manager = enemyManager;
        currentHealth = maxHealth;
        player = enemyManager.GetPlayerTransform();
    }

    public void Activate()
    {
        isActive = true;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        isActive = false;
        gameObject.SetActive(false);
    }

    /// <summary>重生时重置状态：SetActive(false) 会中断闪烁协程，需手动复原颜色和血量。</summary>
    public void ResetState()
    {
        currentHealth = maxHealth;
        isFlashing = false;
        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isActive) return;

        currentHealth -= damage;

        if (!isFlashing)
        {
            StartCoroutine(FlashRed());
        }

        if (hitEffectPrefab != null)
        {
            GameObject hitEffect = Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            Destroy(hitEffect, 2f);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashRed()
    {
        isFlashing = true;

        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = flashColor;
        }

        yield return new WaitForSeconds(flashDuration);

        if (enemyRenderer != null)
        {
            enemyRenderer.material.color = originalColor;
        }

        isFlashing = false;
    }

    void Die()
    {
        if (deathEffectPrefab != null)
        {
            GameObject deathEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(deathEffect, 3f);
        }

        if (onEnemyDeathEvent != null)
            onEnemyDeathEvent.Raise(gameObject);

        if (manager != null)
        {
            manager.RespawnEnemy(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
