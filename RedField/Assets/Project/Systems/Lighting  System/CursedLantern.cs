using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CursedLantern : MonoBehaviour
{
    private Light2D lanternLight;

    [Header("Cấu hình nhấp nháy tâm linh")]
    [SerializeField] private float baseIntensity = 1.3f;     // Cường độ sáng chuẩn
    [SerializeField] private float flickerVariance = 0.15f;  // Độ lệch ánh sáng khi nhấp nháy
    [SerializeField] private float flickerSpeed = 10f;       // Tốc độ nhấp nháy thay đổi liên tục

    private void Awake()
    {
        lanternLight = GetComponentInChildren<Light2D>();
    }

    private void Update()
    {
        if (lanternLight == null) return;

        float noise = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);

        // Chuyển đổi giá trị noise (0 đến 1) thành khoảng dao động quanh baseIntensity
        float calculatedIntensity = baseIntensity + Mathf.Lerp(-flickerVariance, flickerVariance, noise);

        lanternLight.intensity = calculatedIntensity;
    }
}