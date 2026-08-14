using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 20, 0);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float fieldOfView = 60f;
    [SerializeField] private Vector3 fixedRotation = new Vector3(90f, 0f, 0f);

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographic = false;
            cam.fieldOfView = fieldOfView;
        }

        // 俯视相机的朝向是固定的，只在启动时设置一次
        transform.rotation = Quaternion.Euler(fixedRotation);

        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 只跟随位置，不改变朝向（LookAt 会因位置插值滞后而导致相机旋转）
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }
}
