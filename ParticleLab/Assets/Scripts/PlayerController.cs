using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float shootInterval = 0.1f;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private GameEventSO onBulletSpawnEvent;

    private float shootTimer;
    private Vector3 moveDirection;

    void Awake()
    {
        if (bulletPrefab == null)
            Debug.LogError("[PlayerController] Bullet Prefab 未赋值，不会发射任何子弹。", this);
        if (onBulletSpawnEvent == null)
            Debug.LogWarning("[PlayerController] On Bullet Spawn Event 未赋值，子弹不会自动瞄准敌人。", this);
        if (shootInterval <= 0f)
            Debug.LogWarning("[PlayerController] Shoot Interval 为 0，每个物理帧都会生成子弹。", this);
    }

    void Update()
    {
        HandleInput();
        Move();
    }

    void FixedUpdate()
    {
        AutoShoot();
    }

    void HandleInput()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            vertical += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            vertical -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontal -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontal += 1f;

        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
    }

    void Move()
    {
        if (moveDirection.magnitude > 0.1f)
        {
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }

    void AutoShoot()
    {
        shootTimer += Time.fixedDeltaTime;
        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            SpawnBullet();
        }
    }

    void SpawnBullet()
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = bulletSpawnPoint != null ? bulletSpawnPoint.position : transform.position;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

        if (onBulletSpawnEvent != null)
            onBulletSpawnEvent.Raise(bullet);
    }
}
