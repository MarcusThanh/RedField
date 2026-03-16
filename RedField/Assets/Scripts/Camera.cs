using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [SerializeField] private float yHeight = 0f;
    [SerializeField] private float zDepth = -10f;
    [SerializeField] private float smoothSpeed = 5f;

    // Khai báo 2 bức tường thủ công 
    [SerializeField] private float minX; 
    [SerializeField] private float maxX; 

    private void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x;

        targetX = Mathf.Clamp(targetX, minX, maxX);

        float smoothedX = Mathf.Lerp(transform.position.x, targetX, smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(smoothedX, yHeight, zDepth);
    }
}