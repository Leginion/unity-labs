using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 20, 0);
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool useOrthographic = true;
    [SerializeField] private float orthographicSize = 15f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null && useOrthographic)
        {
            cam.orthographic = true;
            cam.orthographicSize = orthographicSize;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
}
