using UnityEngine;

public class BulletDirectionSetter : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float detectionRange = 50f;

    void Awake()
    {
        // player 未赋值时退回自身，避免每次射击都抛 NullReferenceException
        if (player == null)
        {
            player = transform;
            Debug.LogWarning("[BulletDirectionSetter] Player 未赋值，已回退为自身 Transform。", this);
        }

        if (enemyLayer.value == 0)
            Debug.LogWarning("[BulletDirectionSetter] Enemy Layer 未设置，子弹不会瞄准任何敌人。", this);
    }

    public void OnBulletSpawned(GameObject bullet)
    {
        if (bullet == null || player == null) return;

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript == null) return;

        Enemy nearestEnemy = FindNearestEnemy();
        
        if (nearestEnemy != null)
        {
            Vector3 direction = (nearestEnemy.transform.position - bullet.transform.position).normalized;
            bulletScript.SetDirection(direction);
        }
        else
        {
            bulletScript.SetDirection(player.forward);
        }
    }

    Enemy FindNearestEnemy()
    {
        Collider[] hits = Physics.OverlapSphere(player.position, detectionRange, enemyLayer);
        Enemy nearest = null;
        float minDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null && enemy.IsActive)
            {
                float distance = Vector3.Distance(player.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = enemy;
                }
            }
        }

        return nearest;
    }
}
