using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);

    [Header("Smooth Settings")]
    [Range(1f, 10f)]
    public float smoothSpeed = 5f;

    [Header("Camera Bounds")]
    public bool useBounds = true; // Boundary =)))
    public float minX;
    public float maxX;

    void LateUpdate() // T muốn các logic của nhân vật được chạy xong trước rồi mới tới camera
    {
        if (target == null) return;

        // Tính toán vị trí gốc mà camera muốn đi tới
        Vector3 desiredPosition = target.position + offset;

        if (useBounds)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX); // Clamp sẽ kẹp giá trị vào trong khoảng minX và maxX
        }

        // Làm mượt chuyển động của camera bằng cách sử dụng Lerp (tutor trên utube =))) )
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}