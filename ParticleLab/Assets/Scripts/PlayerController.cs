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
        if (bulletPrefab != null)
        {
            Vector3 spawnPos = bulletSpawnPoint != null ? bulletSpawnPoint.position : transform.position;
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            
            if (onBulletSpawnEvent != null)
                onBulletSpawnEvent.Raise(bullet);
        }
    }
}
